namespace Server.Controllers.V1;

/// <summary>
/// User Managment and Authentication or Authorization
/// </summary>
public partial class UsersController : BaseController
{
	#region Fields
	[AutoInject] private readonly IUserServices _userServices;
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
		var serviceResult =
			await _userServices.LoginAsync(requestViewModel, ipAddress: GetIPAddress());

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
	/// Get a new token by refresh token
	/// </summary>
	[HttpPost("RefreshToken/{refreshToken?}")]
	public async Task<ActionResult<Result<LoginResponseViewModel>>> RefreshToken(string refreshToken)
	{
		if (string.IsNullOrWhiteSpace(refreshToken))
		{
			var result =
				new Result<LoginResponseViewModel>();

			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(refreshToken));

			result.AddErrorMessage(errorMessage);

			return result.ApiResult();
		}

		var serviceResult =
			await _userServices.RefreshTokenAsync(token: refreshToken, ipAddress: GetIPAddress());

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
	/// Logout user by refreshToken
	/// </summary>
	[HttpDelete("Logout/{refreshToken?}")]
	public async Task<ActionResult<Result>> LogoutToken(string refreshToken)
	{
		if (string.IsNullOrWhiteSpace(refreshToken))
		{
			var result = new Result();

			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull, nameof(refreshToken));

			result.AddErrorMessage(errorMessage);

			return result.ApiResult();
		}

		var serviceResult = await
			_userServices.LogoutAsync(refreshToken);

		return serviceResult.ApiResult();
	}
	#endregion /HttpDelete

	#region Methods
	[NonAction]
	private string? GetIPAddress()
	{
		var requestHeaders = Request?.Headers;

		if (requestHeaders != null && requestHeaders.ContainsKey("X-Forwarded-For"))
		{
			return Request?.Headers["X-Forwarded-For"];
		}
		else
		{
			return HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
		}
	}
	#endregion /Methods
}