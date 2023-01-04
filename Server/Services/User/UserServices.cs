using Microsoft.AspNetCore.Http;

namespace Services;

public partial class UserServices : BaseServices, IUserServices
{
	#region Fields
	private readonly IMapper _mapper;
	private readonly IEasyCachingProvider _cache;
	private readonly ITokenServices _tokenServices;
	private readonly ILogger<UserServices> _logger;
	private readonly IUserRepository _userRepository;
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<Role> _roleManager;
	private readonly ApplicationSettings _applicationSettings;
	#endregion /Fields

	#region Constractor
	public UserServices
		(IMapper mapper,
		IEasyCachingProvider cache,
		ITokenServices tokenServices,
		ILogger<UserServices> logger,
		IUserRepository userRepository,
		UserManager<User> userManager,
		RoleManager<Role> roleManager,
		IOptions<ApplicationSettings> options) : base()
	{
		_mapper = mapper;
		_cache = cache;
		_tokenServices = tokenServices;
		_logger = logger;
		_userRepository = userRepository;
		_userManager = userManager;
		_roleManager = roleManager;
		_applicationSettings = options.Value;
	}
	#endregion /Constractor

	#region Public Methods
	/// <summary>
	/// Ban or UnBan user by userId
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result> ToggleBanUser(int userId)
	{
		var result = new Result();

		var foundedUser =
			await _userManager.FindByIdAsync(userId: userId.ToString());

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		foundedUser.IsBanned = !foundedUser.IsBanned;

		await _userManager.UpdateSecurityStampAsync(foundedUser);

		await _userRepository.SaveChangesAsync();

		//await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

		string successMessage =
			foundedUser.IsBanned == true
				?
				string.Format(Resources.Messages.SuccessMessages.UserBannedSuccessful)
				:
				string.Format(Resources.Messages.SuccessMessages.UserUnbannedSuccessful);

		result.AddSuccessMessage(successMessage);

		_logger.LogWarning(successMessage, parameters: new List<object>
		{
			userId
		});

		return result;
	}


	/// <summary>
	/// Set user IsDelete field to true in database
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result> UserSoftDeleteAsync(int userId)
	{
		var result = new Result();

		var foundedUser =
			await _userManager.FindByIdAsync(userId.ToString());

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		foundedUser.IsDeleted = true;
		await _userManager.UpdateSecurityStampAsync(foundedUser);

		await _userRepository.SaveChangesAsync();

		//await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.DeleteUserSuccessful);

		result.AddSuccessMessage(successMessage);

		_logger.LogWarning(Resources.Resource.DeleteSuccessful, parameters: new List<object>
		{
			userId
		});

		return result;
	}


	/// <summary>
	/// Generate a new access token by refresh token
	/// </summary>
	/// <param name="refreshToken"></param>
	/// <param name="ipAddress"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result<LoginResponseViewModel>>
		RefreshTokenAsync(string refreshToken, string? ipAddress)
	{
		var result =
			 new Result<LoginResponseViewModel>();

		if (result.IsFailed)
			return result;

		if (!Guid.TryParse(refreshToken, out var inputRefreshToken))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidRefreshToken);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var userLogin =
		  await _userRepository.GetUserLoginsAsync
			(refreshToken: inputRefreshToken, includeUser: true);

		if (userLogin == null || userLogin.IsExpired)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidRefreshToken);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (userLogin.User == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (userLogin.User.IsBanned)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserBanned);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var newRefreshToken = GenerateRefreshToken(ipAddress);
		userLogin.RefreshToken = newRefreshToken.RefreshToken;
		userLogin.CreatedByIp = ipAddress;

		await _userRepository.SaveChangesAsync();

		var claims =
			GenerateClaims(new UserClaims
			{
				Id = userLogin.UserId.ToString() ?? string.Empty
			});

		var expiredTime =
			DateTime.UtcNow.AddMinutes(_applicationSettings.JwtSettings?.TokenExpiresTime ?? 15);

		string jwtToken =
			_tokenServices.GenerateJwtToken
				(securityKey: _applicationSettings.JwtSettings?.SecretKeyForToken,
				claimsIdentity: claims,
				dateTime: expiredTime);

		var response =
			new LoginResponseViewModel()
			{
				Token = jwtToken,
				Username = userLogin!.User?.UserName,
				RefreshToken = newRefreshToken.RefreshToken,
			};

		result.Value = response;

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.RefreshTokenSuccessfull);

		result.AddSuccessMessage(successMessage);

		_logger.LogInformation(Resources.Resource.UserRefreshTokenSuccessfulInformation, parameters: new List<object?>
		{
			userLogin.User?.UserName,
		});

		return result;
	}


	/// <summary>
	/// Remove user refresh token in database
	/// </summary>
	/// <param name="refreshToken"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result> LogoutAsync(string refreshToken)
	{
		var result = new Result();

		if (!Guid.TryParse(refreshToken, out var inputRefreshToken))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidRefreshToken);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var userLogin =
		  await _userRepository.GetUserLoginsAsync
			(refreshToken: inputRefreshToken, includeUser: false);

		if (userLogin == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		_userRepository.DeleteUserLogin(userLogin);

		await _userRepository.SaveChangesAsync();

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.RefreshTokenRevoked);

		result.AddSuccessMessage(successMessage);

		return result;
	}


	/// <summary>
	/// Update a user information in database by admin
	/// </summary>
	/// <param name="requestViewModel"></param>
	/// <param name="adminId"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result> UpdateUserByAdminAsync
		(UpdateUserByAdminRequestViewModel requestViewModel, int? adminId)
	{
		var result = new Result();

		if (result.IsFailed)
		{
			return result;
		}

		if (!adminId.HasValue)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(adminId));

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var adminRoleId =
			await _userRepository.GetUserRoleAsync(adminId.Value);

		if (!adminRoleId.HasValue)
		{
			var errorMessage =
				string.Format(nameof(HttpStatusCode.Unauthorized));

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var user =
			await _userRepository.GetUserById(userId: requestViewModel.UserId!.Value, isTracking: false);

		if (user == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (user.Id != adminId)
		{
			//if (user.RoleId == adminRoleId)
			//{
			//	string errorMessage = string.Format
			//		(Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser);

			//	result.AddErrorMessage(errorMessage);

			//	return result;
			//}
		}

		if (adminRoleId == (int)UserRoleEnum.Admin)
		{
			//if (user.RoleId == (int) UserRoleEnum.SystemAdministrator)
			//{
			//	string errorMessage = string.Format
			//		(Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser);

			//	result.AddErrorMessage(errorMessage);

			//	return result;
			//}
		}


		if (user.UserName != requestViewModel.Username)
		{
			var isPhoneNumberExist =
				await _userRepository.CheckUsernameExist(username: requestViewModel.Username);

			if (isPhoneNumberExist)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.PhoneNumberExist);

				result.AddErrorMessage(errorMessage);

				return result;
			}
		}

		if (user.Email != requestViewModel.Email)
		{
			var isEmailExist =
				await _userRepository.CheckEmailExist(email: requestViewModel.Email);

			if (isEmailExist)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.EmailExist);

				result.AddErrorMessage(errorMessage);

				return result;
			}
		}

		var updatedUser =
			_mapper.Map<User>(source: requestViewModel);

		updatedUser.Id = requestViewModel.UserId!.Value;
		updatedUser.SecurityStamp = Guid.NewGuid().ToString();

		_userRepository.UpdateUserByAdmin(updatedUser);

		await _userRepository.SaveChangesAsync();

		await _cache.RemoveByPrefixAsync($"userId-{updatedUser.Id}");

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.UpdateSuccessful);

		result.AddSuccessMessage(successMessage);

		_logger.LogWarning(Resources.Resource.UpdateSuccessful, parameters: new List<object>
		{
			requestViewModel
		});

		return result;
	}


	/// <summary>
	/// Login a user by username and password
	/// </summary>
	/// <param name="loginRequestViewModel"></param>
	/// <param name="ipAddress"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result<LoginResponseViewModel>>
		LoginAsync(LoginRequestViewModel loginRequestViewModel, string? ipAddress)
	{
		var result =
			new Result<LoginResponseViewModel>();

		if (result.IsFailed == true)
			return result;

		var foundedUser =
			await _userManager.FindByNameAsync(loginRequestViewModel.Username!);

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var isPasswordValid =
			await _userManager.CheckPasswordAsync(foundedUser, loginRequestViewModel.Password!);

		if (!isPasswordValid)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (foundedUser.IsBanned)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserBanned);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var expiredTime =
			DateTime.UtcNow.AddMinutes(_applicationSettings.JwtSettings?.TokenExpiresTime ?? 15);

		var refreshToken = GenerateRefreshToken(ipAddress);

		refreshToken.UserId = foundedUser.Id;

		await _userRepository.AddUserLoginAsync(refreshToken);

		await _userRepository.SaveChangesAsync();

		var claims = GenerateClaims(new UserClaims
		{
			Id = foundedUser.Id.ToString(),
		});

		string token =
			_tokenServices.GenerateJwtToken
				(securityKey: _applicationSettings.JwtSettings?.SecretKeyForToken,
				claimsIdentity: claims,
				dateTime: expiredTime);

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.LoginSuccessful);

		result.AddSuccessMessage(successMessage);

		var response =
			new LoginResponseViewModel()
			{
				Token = token,
				Username = foundedUser.UserName,
				RefreshToken = refreshToken.RefreshToken,
			};

		result.Value = response;

		_logger.LogInformation(Resources.Resource.UserLoginSuccessfulInformation, parameters: new List<object>
		{
			new
			{
				UserName = loginRequestViewModel.Username,
			}
		});

		return result;
	}


	/// <summary>
	/// Login a user by OAuth standard Authentication (for use in swagger ui)
	/// </summary>
	/// <param name="requestViewModel"></param>
	/// <param name="ipAddress"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result<LoginByOAuthResponseViewModel>> LoginByOAuthAsync
		(LoginByOAuthRequestViewModel requestViewModel, string? ipAddress)
	{
		var result =
			new Result<LoginByOAuthResponseViewModel>();

		if (!requestViewModel.Grant_Type?.Equals("password", StringComparison.OrdinalIgnoreCase) == true)
		{
			string errorMessage = "OAuth flow is not password.";

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var foundedUser =
			await _userManager.FindByNameAsync(requestViewModel.Username!);

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var isPasswordValid =
			await _userManager.CheckPasswordAsync(foundedUser, requestViewModel.Password!);

		if (!isPasswordValid)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (foundedUser.IsBanned)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserBanned);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var expiredTime =
			DateTime.UtcNow.AddMinutes(_applicationSettings.JwtSettings?.TokenExpiresTime ?? 15);

		var refreshToken = GenerateRefreshToken(ipAddress);

		refreshToken.UserId = foundedUser.Id;

		await _userRepository.AddUserLoginAsync(refreshToken);

		await _userRepository.SaveChangesAsync();

		var claims = GenerateClaims(new UserClaims
		{
			Id = foundedUser.Id.ToString(),
		});

		string token =
			_tokenServices.GenerateJwtToken
				(securityKey: _applicationSettings.JwtSettings?.SecretKeyForToken,
				claimsIdentity: claims,
				dateTime: expiredTime);

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.LoginSuccessful);

		result.AddSuccessMessage(successMessage);

		var response =
			new LoginByOAuthResponseViewModel()
			{
				access_token = token,
				username = foundedUser.UserName,
				refresh_token = refreshToken.RefreshToken.ToString(),
				token_type = "Bearer",
			};

		result.Value = response;

		_logger.LogInformation(Resources.Resource.UserLoginSuccessfulInformation, parameters: new List<object>
		{
			new
			{
				UserName = requestViewModel.Username,
			}
		});

		return result;
	}


	/// <summary>
	/// Create a new user in database
	/// </summary>
	/// <param name="registerRequestViewModel"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result>
		RegisterAsync(RegisterRequestViewModel registerRequestViewModel)
	{
		var result = new Result();

		var registerUser =
			new User(registerRequestViewModel.Username!)
			{
				Email = registerRequestViewModel.Email,
			};

		var identityUserResult =
			await _userManager.CreateAsync(registerUser, registerRequestViewModel.Password!);

		if (!identityUserResult.Succeeded)
		{
			foreach (var error in identityUserResult.Errors)
			{
				result.AddErrorMessage(error.Description);
			}

			return result;
		}

		var identityRoleResult =
			await _userManager.AddToRoleAsync(registerUser, Constants.Role.User);

		if (!identityRoleResult.Succeeded)
		{
			foreach (var error in identityUserResult.Errors)
			{
				result.AddErrorMessage(error.Description);
			}

			return result;
		}

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.RegisterSuccessful);

		result.AddSuccessMessage(successMessage);

		return result;
	}


	/// <summary>
	/// Change user role by admin
	/// </summary>
	/// <param name="requestViewModel"></param>
	/// <param name="adminId"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result> ChangeUserRoleAsync
		(ChangeUserRoleRequestViewModel requestViewModel, int adminId)
	{
		var result = new Result();

		var adminUser =
			await _userManager.FindByIdAsync(adminId.ToString());

		if (adminUser == null)
		{
			string errorMessage = 
				string.Format(nameof(HttpStatusCode.Unauthorized));

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var adminRoles =
			await _userManager.GetRolesAsync(adminUser);

		var isRoleExist =
			await _roleManager.RoleExistsAsync(requestViewModel.RoleName);

		if (!isRoleExist)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.RoleNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var foundedUser =
			await _userManager.FindByIdAsync(userId: requestViewModel.UserId!.Value.ToString());

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var userRoles =
			await _userManager.GetRolesAsync(foundedUser);

		if (foundedUser.Id == adminId)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (adminRoles[0] == userRoles[0])
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		if (adminRoles[0] == Constants.Role.Admin)
		{
			if (userRoles[0] == Constants.Role.SystemAdmin)
			{
				var errorMessage = string.Format
					(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

				result.AddErrorMessage(errorMessage);

				return result;
			}

			if (requestViewModel.RoleName == Constants.Role.SystemAdmin ||
				requestViewModel.RoleName == Constants.Role.Admin)
			{
				var errorMessage = string.Format
					(Resources.Messages.ErrorMessages.AccessDeniedForChangeThisRole);

				result.AddErrorMessage(errorMessage);

				return result;
			}
		}

		foreach (var role in userRoles)
		{
			await _userManager.RemoveFromRoleAsync(foundedUser, role);
		}

		await _userManager.AddToRoleAsync(foundedUser, requestViewModel.RoleName);

		await _userManager.UpdateSecurityStampAsync(foundedUser);

		//await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.UpdateSuccessful);

		result.AddSuccessMessage(successMessage);

		_logger.LogWarning(Resources.Resource.UpdateSuccessful, parameters: new List<object>
		{
			requestViewModel
		});

		return result;
	}
	#endregion /Public Methods

	#region Private Methods
	/// <summary>
	/// Generate new refresh token
	/// </summary>
	/// <param name="ipAddress"></param>
	/// <returns>Success or Failed Result</returns>
	private UserLogin GenerateRefreshToken(string? ipAddress)
	{
		return new UserLogin(refreshToken: Guid.NewGuid())
		{
			Expires = DateTime.UtcNow.AddDays(30),
			Created = DateTime.UtcNow,
			CreatedByIp = ipAddress
		};
	}


	/// <summary>
	/// Generate a new user claims for authentiction
	/// </summary>
	/// <param name="userClaims"></param>
	/// <returns>Success or Failed Result</returns>
	private ClaimsIdentity GenerateClaims(UserClaims userClaims)
	{
		var claims =
			new ClaimsIdentity(new[]
			{
				new Claim
					(type: ClaimTypes.NameIdentifier, value: userClaims.Id),
			});

		return claims;
	}
	#endregion /Private Methods
}
