using System;

namespace Dtat.Logging;

public interface ILogger<T> where T : class
{
	bool IsEnabled(LogLevel logLevel);

	void Log(LogLevel logLevel, Exception? exception, string? message);

	void Log<T1>(LogLevel logLevel, Exception? exception, string? message, T1 parameter);

	void Log<T1, T2>(LogLevel logLevel, Exception? exception, string? message, T1 parameter1, T2 parameter2);

	void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args);
}

public interface ILogger
{
	bool IsEnabled(LogLevel logLevel); 

	void Log(Type type, LogLevel logLevel, Exception? exception, string? message);

	void Log<T1>(Type type, LogLevel logLevel, Exception? exception, string? message, T1 parameter);

	void Log<T1, T2>(Type type, LogLevel logLevel, Exception? exception, string? message, T1 parameter1, T2 parameter2);

	void Log(Type type, LogLevel logLevel, Exception? exception, string? message, params object?[] args);
}