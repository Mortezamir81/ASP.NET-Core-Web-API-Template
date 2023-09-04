using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Dtat.Logging;

public interface ILogger<T> where T : class
{
	bool IsTraceEnabled { get; set; }
	bool IsDebugEnabled { get; set; }
	bool IsInformationEnabled { get; set; }
	bool IsErrorEnabled { get; set; }
	bool IsCriticalEnabled { get; set; }
	bool IsWarningEnabled { get; set; }

	bool LogTrace
		(string message,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);

	bool LogDebug
		(string message,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);

	bool LogInformation
		(string message,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);

	bool LogWarning
		(string message,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);

	bool LogError
		(Exception exception,
		string? message = null,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);

	bool LogCritical
		(Exception exception,
		string? message = null,
			[CallerMemberName] string? methodName = null,
			List<object?>? parameters = null);
}

public interface ILogger
{
	bool IsTraceEnabled { get; set; }
	bool IsDebugEnabled { get; set; }
	bool IsInformationEnabled { get; set; }
	bool IsErrorEnabled { get; set; }
	bool IsCriticalEnabled { get; set; }
	bool IsWarningEnabled { get; set; }

	bool LogTrace
		(string message,
		Type classType,
		[CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);

	bool LogDebug
		(string message,
		Type classType,
		[CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);

	bool LogInformation
		(string message,
		Type classType,
		[CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);

	bool LogWarning
		(string message,
		Type classType,
		[CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);

	bool LogError
		(Exception exception,
		Type classType,
		string? message = null, [CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);

	bool LogCritical
		(Exception exception,
		Type classType,
		string? message = null, [CallerMemberName] string? methodName = null,
		List<object?>? parameters = null);
}
