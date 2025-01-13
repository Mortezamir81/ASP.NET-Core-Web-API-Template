namespace Services;

public partial class HttpContextService : object
{
	#region Fields
	[AutoInject] private readonly IHttpContextAccessor _httpContextAccessor;
	[AutoInject] private readonly IOptions<ApplicationSettings> _options;
	#endregion /Fields

	#region Methods

	#region GetHttpReferer()
	public string? GetHttpReferrer()
	{
		if (_httpContextAccessor.HttpContext is null)
			return null;

		if (_httpContextAccessor.HttpContext.Request is null)
			return null;

		// using Microsoft.AspNetCore.Http;
		var typedHeaders =
			_httpContextAccessor
			.HttpContext.Request.GetTypedHeaders();

		var result =
			typedHeaders?.Referer?.AbsoluteUri;

		return result;
	}
	#endregion /GetHttpReferer()

	#region GetRemoteIpAddress()
	public string? GetRemoteIpAddress()
	{
		if (_httpContextAccessor.HttpContext is null)
			return null;

		if (_httpContextAccessor.HttpContext.Request is null)
			return null;

		var request =
			_httpContextAccessor.HttpContext.Request;

		var requestHeaders = request.Headers;

		if (requestHeaders != null && requestHeaders.ContainsKey("X-Forwarded-For"))
		{
			return request.Headers["X-Forwarded-For"];
		}

		var remoteIpAddress =
			request.HttpContext.Connection.RemoteIpAddress;

		var result =
			remoteIpAddress?.MapToIPv4().ToString();

		return result;
	}
	#endregion /GetRemoteIpAddress()

	#region GetCurrentHostName()
	/// <summary>
	/// Site Domain Example Name: ChilyWeb.ir
	/// </summary>
	public string? GetCurrentHostName()
	{
		if (_httpContextAccessor.HttpContext is null)
			return null;

		if (_httpContextAccessor.HttpContext.Request is null)
			return null;

		var result =
			_httpContextAccessor?
			.HttpContext?.Request?.Host.Value?.ToLower();

		return result;
	}
	#endregion /GetCurrentHostName()

	#region GetCurrentHostProtocol()
	/// <summary>
	/// Site Protocol: HTTP or HTTPS
	/// </summary>
	public string? GetCurrentHostProtocol()
	{
		if (_httpContextAccessor.HttpContext is null)
			return null;

		if (_httpContextAccessor.HttpContext.Request is null)
			return null;

		var result =
			_httpContextAccessor
			.HttpContext.Request.Scheme;

		return result;
	}
	#endregion /GetCurrentHostProtocol()

	#region GetCurrentHostUrl()
	/// <summary>
	/// Site URL: https://ChilyWeb.ir
	/// </summary>
	public string? GetCurrentHostUrl()
	{
		var currentHostName =
			GetCurrentHostName();

		if (currentHostName is null)
			return null;

		var currentHostProtocol =
			GetCurrentHostProtocol();

		if (currentHostProtocol is null)
			return null;

		var result =
			$"{currentHostProtocol}://{currentHostName}";

		return result;
	}
	#endregion /GetCurrentHostUrl()

	#region GetClientHostUrl()
	/// <summary>
	/// Site URL: https://ChilyWeb.ir
	/// </summary>
	public string GetClientHostUrl()
	{
		var clientUrl =
			_options.Value.ClientUrl;

		return clientUrl ?? "-";
	}
	#endregion /GetCurrentHostUrl()

	#endregion /Methods
}
