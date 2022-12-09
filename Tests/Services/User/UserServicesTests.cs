namespace Tests.Services.User;

public class UserServicesTests : IClassFixture<DatabaseFixture>
{
	#region Fields
	private MockRepository _mockRepository;

	private Mock<IMapper> _mockMapper;
	private Mock<IEasyCachingProvider> _mockEasyCachingProvider;
	private ITokenServices _tokenServices;
	private Mock<ILogger<UserServices>> _mockLogger;
	private Mock<IOptions<ApplicationSettings>> _mockOptions;

	private IUserRepository _userRepository { get; set; }
	#endregion /Fields

	#region Constractor
	public UserServicesTests(DatabaseFixture databaseFixture)
	{
		_mockRepository = new MockRepository(MockBehavior.Loose);

		_mockMapper =
			_mockRepository.Create<IMapper>();

		_mockEasyCachingProvider =
			_mockRepository.Create<IEasyCachingProvider>();

		_mockLogger =
			_mockRepository.Create<ILogger<UserServices>>();

		_mockOptions =
			_mockRepository.Create<IOptions<ApplicationSettings>>();

		_userRepository =
			new UserRepository(databaseFixture.CreateContext());

		_tokenServices =
			new TokenServices(new Mock<ILogger<TokenServices>>().Object, _mockEasyCachingProvider.Object);
	}
	#endregion /Constractor

	#region LoginTests
	[Fact]
	public async Task LoginAsync_PassingNullViewModel_MostNotBeNullError()
	{
		// Arrange
		var userServices = CreateUserServices();

		LoginRequestViewModel loginRequestViewModel = null;

		string? ipAddress = null;

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(loginRequestViewModel));

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, ipAddress);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.Equal(expected: exceptedErrorMessage, result.Errors[0]);
	}


	[Theory]
	[InlineData(null, null)]
	[InlineData("", "")]
	[InlineData("    ", "    ")]
	public async Task LoginAsync_PassingNullOrEmptyUsernameAndPass_MostNotBeNullError(string username, string password)
	{
		// Arrange
		var userServices = CreateUserServices();

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = username,
				Password = password
			};

		string? ipAddress = null;

		var exceptedUsernameErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(loginRequestViewModel.Username));

		var exceptedPasswordErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(loginRequestViewModel.Password));

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, ipAddress);

		var isUsernameErrorExist =
			result.Errors.Any(current => current == exceptedUsernameErrorMessage);

		var isPasswordErrorExist =
			result.Errors.Any(current => current == exceptedPasswordErrorMessage);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsSuccess);
		Assert.True(isUsernameErrorExist);
		Assert.True(isPasswordErrorExist);
	}


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
			result.Errors.Any(current => current == exceptedErrorMessage);

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
				Username = Consts.BanUserUsername,
				Password = Consts.UsersPassword,
			};

		string? ipAddress = null;

		var exceptedErrorMessage =
			string.Format(Resources.Messages.ErrorMessages.UserBanned);

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, ipAddress);

		var isErrorExist =
			result.Errors.Any(current => current == exceptedErrorMessage);

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

		var username = Consts.UserUsername;

		var loginRequestViewModel =
			new LoginRequestViewModel
			{
				Username = username,
				Password = Consts.UsersPassword,
			};

		var exceptedSuccessMessage =
			string.Format(Resources.Messages.SuccessMessages.LoginSuccessful);

		// Act
		var result =
			await userServices.LoginAsync(loginRequestViewModel, null);

		var isSuccessExist =
			result.Successes.Any(current => current == exceptedSuccessMessage);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.True(isSuccessExist);
		Assert.Equal(expected: result.Value.Username, actual: username);
		Assert.NotEmpty(result.Value?.Token);
		Assert.NotEmpty(result.Value.RefreshToken.ToString());
	}
	#endregion /LoginTests

	#region OtherMethods
	private UserServices CreateUserServices()
	{
		_mockOptions
			.SetupGet(current => current.Value)
			.Returns(new ApplicationSettings
			{
				JwtSettings = new JwtSettings
				{
					SecretKeyForEncryptionToken = Guid.NewGuid().ToString(),
					SecretKeyForToken = Guid.NewGuid().ToString(),
					TokenExpiresTime = 60
				}
			});

		return new UserServices
			(_mockMapper.Object,
			_mockEasyCachingProvider.Object,
			_tokenServices,
			_mockLogger.Object,
			_userRepository,
			_mockOptions.Object);
	}
	#endregion /OtherMethods
}
