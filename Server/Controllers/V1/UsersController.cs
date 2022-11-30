using Microsoft.VisualBasic;

namespace Server.Controllers.V1;

/// <summary>
/// User Managment and Authentication or Authorization
/// </summary>
public class UsersController : BaseController
{
	#region Fields
	private readonly IUserServices _userServices;
	#endregion /Fields

	#region Constractor
	public UsersController(IUserServices userServices) : base()
	{
		_userServices = userServices;
	}
	#endregion Constractor

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
	/// Login user with username and password by OAuth2
	/// </summary>
	[HttpPost("LoginByOAuth")]
	public async Task<ActionResult<Result<LoginByOAuthResponseViewModel>>>
		LoginByOAuthAsync([FromForm] LoginByOAuthRequestViewModel requestViewModel)
	{
		var serviceResult =
			await _userServices.LoginByOAuthAsync(requestViewModel, ipAddress: GetIPAddress());

		if (serviceResult.IsFailed)
		{
			return BadRequest();
		}

		return Ok(serviceResult.Value);
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
		var serviceResult =
			await _userServices.RefreshTokenAsync(token: refreshToken, ipAddress: GetIPAddress());

		return serviceResult.ApiResult();
	}
	#endregion /HttpPost

	#region HttpPut
	/// <summary>
	/// Change user role by userId and roleId
	/// </summary>
	[Authorize(Roles = $"{Constants.Role.SystemAdmin},{Constants.Role.Admin}")]
	[LogInputParameter(InputLogLevel.Warning)]
	[HttpPut("ChangeUserRole")]
	public async Task<ActionResult<Result>>
		ChangeUserRoleAsync([FromBody] ChangeUserRoleRequestViewModel changeUserRoleRequestViewModel)
	{
		var serviceResult =
			await _userServices.ChangeUserRoleAsync(changeUserRoleRequestViewModel);

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
		var serviceResult =
			await _userServices.UpdateUserByAdminAsync(viewModel: requestViewModel);

		return serviceResult.ApiResult();
	}
	#endregion /HttpPut

	#region HttpDelete
	/// <summary>
	/// Delete a user by userId
	/// </summary>
	[Authorize(Roles = $"{Constants.Role.SystemAdmin},{Constants.Role.Admin}")]
	[LogInputParameter(InputLogLevel.Warning)]
	[HttpDelete("UserSoftDelete/{userId:long?}")]
	public async Task<ActionResult<Result>> UserSoftDeleteAsync(long? userId)
	{
		var serviceResult =
			await _userServices.UserSoftDeleteAsync(userId: userId);

		return serviceResult.ApiResult();
	}

	/// <summary>
	/// Logout user by refreshToken
	/// </summary>
	[HttpDelete("Logout/{refreshToken?}")]
	public async Task<ActionResult<Result>> LogoutToken(string refreshToken)
	{
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