﻿namespace Dtat.Results.Server;

public enum MessageCodes : int
{
	HttpSuccessCode = 200,
	HttpBadRequestCode = 400,
	HttpServerError = 500,
	HttpNotFoundError = 404,
	HttpUnauthorizeError = 401,
	HttpForbidenError = 403,
}