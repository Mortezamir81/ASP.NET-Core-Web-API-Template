using System.Collections.Generic;
using System.Text.Json;

namespace Dtat.Logging;

public class LogModel : object
{
	public LogModel(LogLevel logLevel) : base()
	{
		LogLevel = logLevel;
	}

	public LogLevel LogLevel { get; set; }

	public string? ApplicationName { get; set; }


	public string? Namespace { get; set; }

	public string? ClassName { get; set; }

	public string? MethodName { get; set; }



	public string? LocalIP { get; set; }

	public string? LocalPort { get; set; }

	public string? RemoteIP { get; set; }

	public string? UserName { get; set; }

	public string? RequestPath { get; set; }

	public string? HttpReferrer { get; set; }



	public string? Message { get; set; }

	public object?[]? Parameters { get; set; }

	public List<ExceptionModel>? Exceptions { get; set; }


	public override string ToString()
	{
		// Provide a simple, fast, and safe summary for debugging purposes.
		return $"[{LogLevel}] {Message}";
	}

	public void Reset(LogLevel logLevel)
	{
		LogLevel = logLevel;
		ApplicationName = null;
		Namespace = null;
		ClassName = null;
		MethodName = null;
		LocalIP = null;
		LocalPort = null;
		RemoteIP = null;
		UserName = null;
		RequestPath = null;
		HttpReferrer = null;
		Message = null;
		Parameters = null;
		Exceptions?.Clear();
	}

	public void Clear()
	{
		ApplicationName = null;
		Namespace = null;
		ClassName = null;
		MethodName = null;
		LocalIP = null;
		LocalPort = null;
		RemoteIP = null;
		UserName = null;
		RequestPath = null;
		HttpReferrer = null;
		Message = null;
		Parameters = null;
		Exceptions = null;
	}
}

public class ExceptionModel
{
	public ExceptionModel(string message)
	{
		Message = message;
	}

	public string Message { get; set; }

	public string? StackTrace { get; set; }
}
