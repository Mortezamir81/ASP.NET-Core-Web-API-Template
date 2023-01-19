using System.Net;

namespace Tests.Services.User;

public class UserServicesTests : IClassFixture<RegistrationServices>
{
	#region Fields
	private MockRepository _mockRepository;
	private Mock<IMapper> _mockMapper;
	private Mock<IEasyCachingProvider> _mockEasyCachingProvider;
	private Mock<Dtat.Logging.ILogger<UserServices>> _mockLogger;

	private IOptions<ApplicationSettings> _options;
	private IUserRepository _userRepository { get; set; }
	private ITokenServices _tokenServices;

	private UserManager<Domain.UserManagment.User> _userManager;
	private RoleManager<Role> _roleManager;
	#endregion /Fields

	#region Constractor
	public UserServicesTests(RegistrationServices registrationServices)
	{
		var databaseContext =
			registrationServices.ServiceProvider.GetRequiredService<DatabaseContext>();

		_mockRepository = new MockRepository(MockBehavior.Loose);

		_mockMapper =
			_mockRepository.Create<IMapper>();

		_mockEasyCachingProvider =
			_mockRepository.Create<IEasyCachingProvider>();

		_mockLogger =
			_mockRepository.Create<Dtat.Logging.ILogger<UserServices>>();

		_userRepository =
			new UserRepository(databaseContext);

		_tokenServices = new TokenServices();

		_options =
			registrationServices.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();

		_userManager =
			registrationServices.ServiceProvider.GetRequiredService<UserManager<Domain.UserManagment.User>>();

		_roleManager =
			registrationServices.ServiceProvider.GetRequiredService<RoleManager<Domain.UserManagment.Role>>();
	}
	#endregion /Constractor

	#region LoginTests
	[Fact]
	public async Task LoginAsync_PassingNotFoundUsername_UserNotFoundError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString()
			};

		string? ipAddress = null;

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.InvalidUserAndOrPass);

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, ipAddress);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task LoginAsync_PassingBannedUsername_UserBannedError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = Consts.UserServices.BanUserUsername,
				Password = Consts.UserServices.UsersPassword,
			};

		string? ipAddress = null;

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.UserBanned);

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, ipAddress);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task LoginAsync_PassingCorrectUsername_SuccessMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var username = Consts.UserServices.UserUsername;

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = username,
				Password = Consts.UserServices.UsersPassword,
			};

		var exceptedSuccessMessage =
			string.Format(Resources.Messages.SuccessMessages.LoginSuccessful);

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, null);

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
		Assert.Equal(expected: result.Value.Username, actual: username);
		Assert.NotEmpty(result.Value?.Token);
		Assert.NotEmpty(result.Value.RefreshToken.ToString());
	}
	#endregion /LoginTests

	#region RegisterTests
	[Fact]
	public async Task RegisterAsync_PassingPasswordLessThan8Char_IdentityPasswordError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var registerRequestViewModel =
			new RegisterRequestViewModel
			{
				Username = Guid.NewGuid().ToString(),
				Email = $"{Guid.NewGuid().ToString()[0..10]}@gmail.com",
				Password = "1234567"
			};

		var exceptedErrorMessage =
			new IdentityErrorDescriber()
			.PasswordTooShort(_options.Value.IdentitySettings.PasswordRequiredLength)
			.Description;

		// Act
		var result =
			await userServices.RegisterAsync(registerRequestViewModel);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task RegisterAsync_PassingExistingUsername_IdentityUsernameExistError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var registerRequestViewModel =
			new RegisterRequestViewModel
			{
				Username = Consts.UserServices.UserUsername,
				Email = $"{Guid.NewGuid().ToString()[0..10]}@gmail.com",
				Password = Consts.UserServices.UsersPassword
			};

		var exceptedErrorMessage =
			new IdentityErrorDescriber()
			.DuplicateUserName(registerRequestViewModel.Username)
			.Description;

		// Act
		var result =
			await userServices.RegisterAsync(registerRequestViewModel);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task RegisterAsync_PassingInvalidEmail_IdentityInvalidEmailError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var registerRequestViewModel =
			new RegisterRequestViewModel
			{
				Username = Guid.NewGuid().ToString(),
				Email = Guid.NewGuid().ToString(),
				Password = Consts.UserServices.UsersPassword
			};

		var exceptedErrorMessage =
			new IdentityErrorDescriber()
			.InvalidEmail(registerRequestViewModel.Email)
			.Description;

		// Act
		var result =
			await userServices.RegisterAsync(registerRequestViewModel);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task RegisterAsync_PassingExistingEmail_IdentityEmailExistError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var registerRequestViewModel =
			new RegisterRequestViewModel
			{
				Username = Guid.NewGuid().ToString(),
				Email = Consts.UserServices.AdminEmail,
				Password = Consts.UserServices.UsersPassword
			};

		var exceptedErrorMessage =
			new IdentityErrorDescriber()
			.DuplicateEmail(registerRequestViewModel.Email)
			.Description;

		// Act
		var result =
			await userServices.RegisterAsync(registerRequestViewModel);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task RegisterAsync_PassingValidInformation_IdentitySuccess()
	{
		// Arrange
		var userServices = CreateUserServices();

		var registerRequestViewModel =
			new RegisterRequestViewModel
			{
				Username = Guid.NewGuid().ToString(),
				Email = $"{Guid.NewGuid().ToString().Replace("-", "").AsSpan().Slice(0, 10)}@gmail.com",
				Password = Consts.UserServices.UsersPassword
			};

		var exceptedSuccessMessage =
			Resources.Messages.SuccessMessages.RegisterSuccessful;

		// Act
		var result =
			await userServices.RegisterAsync(registerRequestViewModel);

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
	}
	#endregion /RegisterTests

	#region RefreshTokenTests
	[Fact]
	public async Task RefreshTokenAsync_PassingInvalidRefreshToken_InvalidRefreshTokenError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.InvalidRefreshToken;

		// Act
		var result =
			await userServices.RefreshTokenAsync("12345", "123");

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task RefreshTokenAsync_PassingValidRefreshToken_RefreshTokenSuccessful()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedSuccessMessage =
			Resources.Messages.SuccessMessages.RefreshTokenSuccessfull;

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = Consts.UserServices.AdminUsername,
				Password = Consts.UserServices.UsersPassword
			};

		var userLogined =
			await userServices.LoginAsync(loginRequestViewModel, null);

		// Act
		var result =
			await userServices.RefreshTokenAsync(userLogined.Value.RefreshToken.ToString(), null);

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.NotNull(result.Value.Token);
		Assert.NotNull(result.Value.Username);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
	}
	#endregion /RefreshTokenTests

	#region ToggleBanUserTests
	[Fact]
	public async Task ToggleBanUserAsync_PassingInvalidUserId_UserNotFoundError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.UserNotFound;

		// Act
		var result =
			await userServices.ToggleBanUser(int.MaxValue);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ToggleBanUserAsync_PassingValidUserId_SuccessMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedSuccessfullyBanedMessage =
			Resources.Messages.SuccessMessages.UserBannedSuccessful;

		var exceptedSuccessfullyUnBanedMessage =
			Resources.Messages.SuccessMessages.UserUnbannedSuccessful;

		// Act
		var resultBanUser =
			await userServices.ToggleBanUser(Consts.UserServices.UserId);

		var isSuccessBanExist =
			resultBanUser.Messages.Any(current => current == exceptedSuccessfullyBanedMessage);


		var resultUnBanUser =
			await userServices.ToggleBanUser(Consts.UserServices.UserId);

		var isSuccessUnBanExist =
			resultUnBanUser.Messages.Any(current => current == exceptedSuccessfullyUnBanedMessage);

		// Assert
		Assert.NotNull(resultBanUser);
		Assert.True(resultBanUser.IsSuccess);
		Assert.True(resultUnBanUser.IsSuccess);
		Assert.True(isSuccessBanExist);
		Assert.True(isSuccessUnBanExist);
	}
	#endregion /ToggleBanUserTests

	#region UserSoftDeleteTests
	[Fact]
	public async Task UserSoftDeleteAsync_PassingInvalidUserId_UserNotFoundError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.UserNotFound;

		// Act
		var result =
			await userServices.UserSoftDeleteAsync(int.MaxValue);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UserSoftDeleteAsync_PassingValidUserId_SuccessMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedSuccessMessage =
			Resources.Messages.SuccessMessages.DeleteUserSuccessful;

		// Act
		var result =
			await userServices.UserSoftDeleteAsync(Consts.UserServices.UserForDeleteId);

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
	}
	#endregion /UserSoftDeleteTests

	#region LogoutTests
	[Fact]
	public async Task LogoutAsync_PassingInvalidRefreshToken_InvalidJWTError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.InvalidRefreshToken;

		// Act
		var result =
			await userServices.LogoutAsync("12345");

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task LogoutAsync_PassingNotFoundRefreshToken_UserNotFoundMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.UserNotFound;

		// Act
		var result =
			await userServices.LogoutAsync(Guid.NewGuid().ToString());

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task LogoutAsync_PassingValidRefreshToken_LogoutSuccessfulMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = Consts.UserServices.UserUsername,
				Password = Consts.UserServices.UsersPassword
			};

		var userLogin =
			await userServices.LoginAsync(loginRequestViewModel, null);

		var exceptedSuccessMessage =
			Resources.Messages.SuccessMessages.RefreshTokenRevoked;

		// Act
		var result =
			await userServices.LogoutAsync(userLogin.Value.RefreshToken.ToString());

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
	}
	#endregion /LogoutTests

	#region ChangeUserRoleTests
	[Fact]
	public async Task ChangeUserRoleAsync_PassingInvalidAdminId_UnauthroizeError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			string.Format(nameof(HttpStatusCode.Unauthorized));

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(null, 0);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ChangeUserRoleAsync_PassingInvalidRole_RoleNotFoundError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new ChangeUserRoleRequestViewModel
			{
				RoleName = Guid.NewGuid().ToString(),
				UserId = null
			};

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.RoleNotFound);

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(requestViewModel, Consts.UserServices.SystemAdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ChangeUserRoleAsync_PassingInvalidUser_UserNotFoundError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new ChangeUserRoleRequestViewModel
			{
				RoleName = Constants.Role.User,
				UserId = 0
			};

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.UserNotFound);

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(requestViewModel, Consts.UserServices.SystemAdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ChangeUserRoleAsync_PassingAdminSelf_AccessDeniedError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new ChangeUserRoleRequestViewModel
			{
				RoleName = Constants.Role.User,
				UserId = Consts.UserServices.SystemAdminId
			};

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(requestViewModel, Consts.UserServices.SystemAdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ChangeUserRoleAsync_PassingSystemicSystemAdmin_AccessDeniedError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new ChangeUserRoleRequestViewModel
			{
				RoleName = Constants.Role.User,
				UserId = Consts.UserServices.SystemAdminId
			};

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.AccessDeniedForChangeRole);

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(requestViewModel, Consts.UserServices.SecoundSystemAdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task ChangeUserRoleAsync_PassingValidData_SuccessMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new ChangeUserRoleRequestViewModel
			{
				RoleName = Constants.Role.Admin,
				UserId = Consts.UserServices.UserForEditRoleId
			};

		var exceptedSuccessMessage =
			string.Format(Resources.Messages.SuccessMessages.UpdateSuccessful);

		// Act
		var result =
			await userServices.ChangeUserRoleAsync(requestViewModel, Consts.UserServices.SystemAdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isErrorExist);
	}
	#endregion /ChangeUserRoleTests

	#region UpdateUserByAdminTests
	[Fact]
	public async Task UpdateUserByAdminAsync_PassingInvalidAdminId_UnauthorizeError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var exceptedErrorMessage =
			nameof(HttpStatusCode.Unauthorized);

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(null, 0);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UpdateUserByAdminAsync_PassingNotSystemicAdminAndPeerUser_AccessDeniedError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new UpdateUserByAdminRequestViewModel
			{
				UserId = Consts.UserServices.SecoundAdminId,
			};

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser;

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(requestViewModel, Consts.UserServices.AdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UpdateUserByAdminAsync_PassingHigherLevelUser_AccessDeniedError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new UpdateUserByAdminRequestViewModel
			{
				UserId = Consts.UserServices.SystemAdminId,
			};

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.AccessDeniedForUpdateThisUser;

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(requestViewModel, Consts.UserServices.AdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UpdateUserByAdminAsync_PassingExistUserName_UserNameExistError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new UpdateUserByAdminRequestViewModel
			{
				UserId = Consts.UserServices.UserId,
				Username = Consts.UserServices.AdminUsername
			};

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.UsernameExist;

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(requestViewModel, Consts.UserServices.AdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UpdateUserByAdminAsync_PassingExistEmail_EmailExistError()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new UpdateUserByAdminRequestViewModel
			{
				UserId = Consts.UserServices.UserId,
				Email = Consts.UserServices.AdminEmail,
			};

		var exceptedErrorMessage =
			Resources.Messages.ErrorMessages.EmailExist;

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(requestViewModel, Consts.UserServices.AdminId);

		var isErrorExist =
			result.Messages.Any(current => current == exceptedErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isErrorExist);
	}


	[Fact]
	public async Task UpdateUserByAdminAsync_PassingValidData_SuccessMessage()
	{
		// Arrange
		var userServices = CreateUserServices();

		var requestViewModel =
			new UpdateUserByAdminRequestViewModel
			{
				UserId = Consts.UserServices.UserForUpdateId,
				Email = $"{Guid.NewGuid().ToString()[0..10]}@gmail.com",
				Username = Guid.NewGuid().ToString().Replace("-", "")[0..10],
				FullName = Guid.NewGuid().ToString()
			};

		var exceptedSuccessMessage =
			Resources.Messages.SuccessMessages.UpdateSuccessful;

		// Act
		var result =
			await userServices.UpdateUserByAdminAsync(requestViewModel, Consts.UserServices.AdminId);

		var isSuccessExist =
			result.Messages.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
	}
	#endregion /UpdateUserByAdminTests

	#region OtherMethods
	private UserServices CreateUserServices()
	{
		return new UserServices
			(_mockMapper.Object,
			_mockEasyCachingProvider.Object,
			_tokenServices,
			_mockLogger.Object,
			_userRepository,
			options: _options,
			userManager: _userManager,
			roleManager: _roleManager);
	}
	#endregion /OtherMethods
}
