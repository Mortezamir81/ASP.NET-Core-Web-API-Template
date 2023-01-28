using Dtat.Result;
using System.Security.Claims;
using ViewModels.General;

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

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

			return result;
		}

		if (foundedUser.IsSystemic)
		{
			var errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDenied);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

			return result;
		}

		foundedUser.IsBanned = !foundedUser.IsBanned;

		await _userManager.UpdateSecurityStampAsync(foundedUser);

		await _userRepository.SaveChangesAsync();

		await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

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

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

			return result;
		}

		if (foundedUser.IsSystemic)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDenied);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

			return result;
		}

		foundedUser.IsDeleted = true;
		await _userManager.UpdateSecurityStampAsync(foundedUser);

		await _userRepository.SaveChangesAsync();

		await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

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
				(Resources.Messages.ErrorMessages.InvalidRefreshToken);

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

		var newRefreshToken = Guid.NewGuid();

		userLogin.RefreshToken = newRefreshToken;
		userLogin.CreatedByIp = ipAddress;

		await _userRepository.SaveChangesAsync();

		var claimsIdentity =
			await CreateClaimsIdentity(user: userLogin.User);

		var accessToken =
			CreateAccessToken(claimsIdentity: claimsIdentity);

		var response =
			new LoginResponseViewModel()
			{
				Token = accessToken,
				Username = userLogin!.User?.UserName,
				RefreshToken = newRefreshToken,
			};

		result.Value = response;

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.RefreshTokenSuccessfull);

		result.AddSuccessMessage(successMessage);

		await AddUserLoggedInToCache(userLogin.User!.Id);

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

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

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
		(UpdateUserByAdminRequestViewModel requestViewModel, int adminId)
	{
		var result = new Result();

		var adminUser =
			await _userManager.FindByIdAsync(adminId.ToString());

		if (adminUser == null)
		{
			var errorMessage =
				string.Format(nameof(HttpStatusCode.Unauthorized));

			result.AddErrorMessage(errorMessage, MessageCodes.HttpUnauthorizeError);

			return result;
		}

		var adminRoles =
			await _userManager.GetRolesAsync(adminUser);

		if (adminRoles == null || adminRoles.Count == 0)
		{
			var errorMessage =
				string.Format(nameof(HttpStatusCode.Unauthorized));

			result.AddErrorMessage(errorMessage, MessageCodes.HttpUnauthorizeError);

			return result;
		}

		var user =
			await _userManager.FindByIdAsync(requestViewModel.UserId.ToString()!);

		if (user == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

			return result;
		}

		var userRoles =
			await _userManager.GetRolesAsync(user);

		if (user.Id != adminId)
		{
			if (!adminUser.IsSystemic && userRoles[0] == adminRoles[0])
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser);

				result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

				return result;
			}

			if (adminRoles[0] == Constants.Role.Admin && userRoles[0] == Constants.Role.SystemAdmin)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser);

				result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

				return result;
			}
		}

		if (user.UserName != requestViewModel.Username)
		{
			var isUsernameExist =
				await _userRepository.CheckUsernameExist
					(username: _userManager.NormalizeName(requestViewModel.Username));

			if (isUsernameExist)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.UsernameExist);

				result.AddErrorMessage(errorMessage);

				return result;
			}
		}

		if (user.Email != requestViewModel.Email)
		{
			var isEmailExist =
				await _userRepository.CheckEmailExist
					(email: _userManager.NormalizeEmail(requestViewModel.Email));

			if (isEmailExist)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.EmailExist);

				result.AddErrorMessage(errorMessage);

				return result;
			}
		}

		user.Email = requestViewModel.Email;
		user.UserName = requestViewModel.Username;
		user.FullName = requestViewModel.FullName;

		await _userRepository.SaveChangesAsync();

		await _userManager.UpdateSecurityStampAsync(user);

		await _cache.RemoveByPrefixAsync($"userId-{user.Id}");

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
	/// <param name="requestViewModel"></param>
	/// <param name="ipAddress"></param>
	/// <returns>Success or Failed Result</returns>
	public async Task<Result<LoginResponseViewModel>>
		LoginAsync(LoginRequestViewModel requestViewModel, string? ipAddress)
	{
		var result =
			new Result<LoginResponseViewModel>();

		var foundedUser =
			await GetUserByName(requestViewModel.Username!);

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var isPasswordValid =
			await CheckPasswordValid(user: foundedUser, password: requestViewModel.Password!);

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

		var refreshToken =
			await AddRefreshTokenToDB(userId: foundedUser.Id, ipAddress: ipAddress);

		var claimsIdentity =
			await CreateClaimsIdentity(user: foundedUser);

		var accessToken =
			CreateAccessToken(claimsIdentity: claimsIdentity);

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.LoginSuccessful);

		result.AddSuccessMessage(successMessage);

		var response =
			new LoginResponseViewModel()
			{
				Token = accessToken,
				Username = foundedUser.UserName,
				RefreshToken = refreshToken,
			};

		result.Value = response;

		await AddUserLoggedInToCache(foundedUser.Id);

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
			await GetUserByName(requestViewModel.Username!);

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

			result.AddErrorMessage(errorMessage);

			return result;
		}

		var isPasswordValid =
			await CheckPasswordValid(user: foundedUser, password: requestViewModel.Password!);

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

		var refreshToken =
			await AddRefreshTokenToDB(userId: foundedUser.Id, ipAddress: ipAddress);

		var claimsIdentity =
			await CreateClaimsIdentity(user: foundedUser);

		var accessToken =
			CreateAccessToken(claimsIdentity: claimsIdentity);

		string successMessage = string.Format
			(Resources.Messages.SuccessMessages.LoginSuccessful);

		result.AddSuccessMessage(successMessage);

		var response =
			new LoginByOAuthResponseViewModel()
			{
				access_token = accessToken,
				username = foundedUser.UserName,
				refresh_token = refreshToken.ToString(),
				token_type = "Bearer",
			};

		result.Value = response;

		await AddUserLoggedInToCache(foundedUser.Id);

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
			var errorMessage =
				string.Format(nameof(HttpStatusCode.Unauthorized));

			result.AddErrorMessage(errorMessage, MessageCodes.HttpUnauthorizeError);

			return result;
		}

		var isRoleExist =
			await _roleManager.RoleExistsAsync(requestViewModel.RoleName);

		if (!isRoleExist)
		{
			var errorMessage = string.Format
				(Resources.Messages.ErrorMessages.RoleNotFound);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

			return result;
		}

		var adminRoles =
			await _userManager.GetRolesAsync(adminUser);

		var foundedUser =
			await _userManager.FindByIdAsync
				(userId: requestViewModel.UserId!.Value.ToString());

		if (foundedUser == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UserNotFound);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpNotFoundError);

			return result;
		}

		var userRoles =
			await _userManager.GetRolesAsync(foundedUser);

		if (adminRoles.Count == 0 || userRoles.Count == 0)
		{
			var errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

			return result;
		}

		if (foundedUser.Id == adminId)
		{
			var errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

			return result;
		}

		if (foundedUser.IsSystemic)
		{
			var errorMessage = string.Format
				(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

			result.AddErrorMessage(errorMessage, MessageCodes.HttpForbidenError);

			return result;
		}

		foreach (var role in userRoles)
		{
			await _userManager.RemoveFromRoleAsync(foundedUser, role);
		}

		var addToRoleResult =
			await _userManager.AddToRoleAsync(foundedUser, requestViewModel.RoleName);

		if (!addToRoleResult.Succeeded)
		{
			foreach (var error in addToRoleResult.Errors)
			{
				result.AddErrorMessage(error.Description);
			}

			return result;
		}

		await _userManager.UpdateSecurityStampAsync(foundedUser);

		await _cache.RemoveByPrefixAsync($"userId-{foundedUser.Id}");

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

	private string CreateAccessToken(ClaimsIdentity claimsIdentity)
	{
		var expiredTime =
			DateTime.UtcNow.AddMinutes(_applicationSettings.JwtSettings?.TokenExpiresTime ?? 15);

		var accessToken =
			_tokenServices.GenerateJwtToken
				(securityKey: _applicationSettings.JwtSettings?.SecretKeyForToken,
				claimsIdentity: claimsIdentity,
				dateTime: expiredTime);

		return accessToken;
	}

	private async Task<User?> GetUserByName(string name)
	{
		return await _userManager.FindByNameAsync(name);
	}

	private async Task<bool> CheckPasswordValid(string password, User user)
	{
		return await _userManager.CheckPasswordAsync(user: user, password: password);
	}

	private async Task<Guid> AddRefreshTokenToDB(int userId, string? ipAddress)
	{
		var refreshToken = GenerateRefreshToken(ipAddress);

		refreshToken.UserId = userId;

		await _userRepository.AddUserLoginAsync(refreshToken);

		await _userRepository.SaveChangesAsync();

		return refreshToken.RefreshToken;
	}

	private async Task<ClaimsIdentity> CreateClaimsIdentity(User user)
	{
		var userRoles =
			await _userManager.GetRolesAsync(user);

		var claims = new List<Claim>()
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
			new Claim(ClaimTypes.Name, user.UserName!),
			new Claim(ClaimTypes.Email, user.Email!),
			new Claim(nameof(User.SecurityStamp), user.SecurityStamp!),
		};

		foreach (var userRole in userRoles)
		{
			claims.Add(new Claim(ClaimTypes.Role, userRole));
		}

		var claimIdentity = new ClaimsIdentity(claims);

		return claimIdentity;
	}

	private async Task AddUserLoggedInToCache(int userId)
	{
		await _cache.TrySetAsync
			($"userId-{userId}-loggedin", true, TimeSpan.FromHours(1));
	}
	#endregion /Private Methods
}
