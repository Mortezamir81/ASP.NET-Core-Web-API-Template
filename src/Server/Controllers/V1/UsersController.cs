namespace Server.Controllers.V1;

/// <summary>
/// User Managment and Authentication or Authorization
/// </summary>
public partial class UsersController : BaseController
{
	#region Fields
	[AutoInject] private readonly IUserServices _userServices;
	[AutoInject] private readonly HttpContextService _httpContextService;
	#endregion /Fields

	#region HttpGet

	#endregion /HttpGet

	#region HttpPost
	/// <summary>
	/// Login user with username and password
	/// </summary>
	[HttpPost("Login")]
	public virtual async Task<ActionResult<Result<LoginResponseViewModel>>>
		LoginAsync([FromBody] LoginRequestViewModel requestViewModel)
	{
		var userIP =
			_httpContextService.GetRemoteIpAddress();

		var serviceResult =
			await _userServices.LoginAsync(requestViewModel, ipAddress: userIP);

		return serviceResult.ApiResult();
	}


	/// <summary>
	/// Create a new User
	/// </summary>
	[HttpPost("Register")]
	public async Task<ActionResult<Result>>
		RegisterAccount([FromBody] RegisterRequestViewModel registerRequestViewModel)
	{
		var serviceResult =
			await _userServices.RegisterAsync(registerRequestViewModel: registerRequestViewModel);

		return serviceResult.ApiResult();
	}


	/// <summary>
	/// Refresh Token
	/// </summary>
	[HttpPost("RefreshToken")]
	public async Task<ActionResult<LoginResponseViewModel>>
		RegisterAccount(string refreshToken)
	{
		var userIP =
			_httpContextService.GetRemoteIpAddress();

		var serviceResult =
			await _userServices.RefreshTokenAsync(requestedRefreshToken: refreshToken, userIP);

		return serviceResult.ApiResult();
	}


	/// <summary>
	/// Send forgot password email
	/// </summary>
	[HttpPost("ForgotPassword")]
	public async Task<ActionResult<Result>>
		ForgotPassword(ForgotPasswordRequestViewModel requestViewModel)
	{
		var userIP =
			_httpContextService.GetRemoteIpAddress();

		var siteUrl =
			_httpContextService.GetClientHostUrl();

		var serviceResult =
			await _userServices.ForgotPasswordAsync
			(requestViewModel: requestViewModel, siteUrl: siteUrl, userIpAddress: userIP);

		return serviceResult.ApiResult();
	}
	#endregion /HttpPost

	#region HttpPut
	/// <summary>
	/// Change user role by userId and roleId
	/// </summary>
	[Authorize(Roles = $"{Constants.Role.SystemAdmin}")]
	[LogInputParameter(InputLogLevel.Warning)]
	[HttpPut("ChangeUserRole")]
	public async Task<ActionResult<Result>>
		ChangeUserRoleAsync([FromBody] ChangeUserRoleRequestViewModel requestViewModel)
	{
		var adminId = GetRequierdUserId();

		var serviceResult =
			await _userServices.ChangeUserRoleAsync(requestViewModel, adminId: adminId);

		return serviceResult.ApiResult();
	}


	/// <summary>
	/// Ban or UnBan user by admin
	/// </summary>
	[Authorize(Roles = $"{Constants.Role.SystemAdmin},{Constants.Role.Admin}")]
	[LogInputParameter(InputLogLevel.Warning)]
	[HttpPut("ToggleBanUser")]
	public async Task<ActionResult<Result>>
		ToggleBanUserAsync(ToggleBanUserRequestViewModel requestViewModel)
	{
		var serviceResult =
			await _userServices.ToggleBanUser(userId: requestViewModel.UserId!.Value);

		return serviceResult.ApiResult();
	}


	/// <summary>
	/// Update user informations by admin (by userId)
	/// </summary>
	[LogInputParameter(InputLogLevel.Warning)]
	[Authorize(Roles = $"{Constants.Role.SystemAdmin},{Constants.Role.Admin}")]
	[HttpPut("UpdateUserByAdmin")]
	public async Task<ActionResult>
		UpdateUserByAdminAsync(UpdateUserByAdminRequestViewModel requestViewModel)
	{
		var adminId = GetRequierdUserId();

		var serviceResult =
			await _userServices.UpdateUserByAdminAsync(viewModel: requestViewModel, adminId: adminId);

		return serviceResult.ApiResult();
	}
	#endregion /HttpPut

	#region HttpDelete
	/// <summary>
	/// Delete a user by system admin
	/// </summary>
	[Authorize(Roles = $"{Constants.Role.SystemAdmin}")]
	[LogInputParameter(InputLogLevel.Warning)]
	[HttpDelete("UserDelete")]
	public async Task<ActionResult<Result>>
		UserSoftDeleteAsync([FromQuery] UserSoftDeleteRequestViewModel requestViewModel)
	{
		var serviceResult =
			await _userServices.UserSoftDeleteAsync(userId: requestViewModel.UserId!.Value);

		return serviceResult.ApiResult();
	}

	/// <summary>
	/// Logout user
	/// </summary>
	[HttpDelete("Logout")]
	public async Task<ActionResult<Result>> LogoutToken()
	{
		var userId = User.GetUserId();

		if (!userId.HasValue)
			return Ok();

		var userTokenId =
			User.Claims.FirstOrDefault(c => c.Type == Constants.Authentication.UserTokenId)?.Value;

		var serviceResult = await
			_userServices.LogoutAsync(userTokenId: int.Parse(userTokenId!), userId: userId.Value);

		return serviceResult.ApiResult();
	}
	#endregion /HttpDelete
}