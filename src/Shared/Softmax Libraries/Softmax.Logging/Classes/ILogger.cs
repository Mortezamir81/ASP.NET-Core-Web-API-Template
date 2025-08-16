using System;

namespace Dtat.Logging;

public interface ILogger<T> where T : class
{
	bool IsTraceEnabled { get; }
	bool IsDebugEnabled { get; }
	bool IsInformationEnabled { get; }
	bool IsWarningEnabled { get; }
	bool IsErrorEnabled { get; }
	bool IsCriticalEnabled { get; }

	void LogTrace(
		string message,
		params object?[] args);

	void LogDebug(
		string message,
		params object?[] args);

	void LogInformation(
		string message,
		params object?[] args);

	void LogWarning(
		string message,
		params object?[] args);

	void LogError(
		Exception exception,
		string message,
		params object?[] args);

	void LogCritical(
		Exception exception,
		string message,
		params object?[] args);
}

public interface ILogger
{
	bool IsTraceEnabled { get; }
	bool IsDebugEnabled { get; }
	bool IsInformationEnabled { get; }
	bool IsWarningEnabled { get; }
	bool IsErrorEnabled { get; }
	bool IsCriticalEnabled { get; }

	void LogTrace(
		Type classType,
		string message,
		params object?[] args);

	void LogDebug(
		Type classType,
		string message,
		params object?[] args);

	void LogInformation(
		Type classType,
		string message,
		params object?[] args);

	void LogWarning(
		Type classType,
		string message,
		params object?[] args);

	void LogError(
		Exception exception,
		Type classType,
		string message,
		params object?[] args);

	void LogCritical(
		Exception exception,
		Type classType,
		string message,
		params object?[] args);
}