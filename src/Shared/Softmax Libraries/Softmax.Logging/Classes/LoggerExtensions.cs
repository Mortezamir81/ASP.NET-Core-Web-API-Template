using System;

namespace Dtat.Logging;

public static class LoggerExtensions
{
	// --- Trace ---
	public static void LogTrace<T>(this ILogger<T> logger, string message) where T : class => logger.Log(LogLevel.Trace, null, message);
	public static void LogTrace<T, T1>(this ILogger<T> logger, string message, T1 arg1) where T : class => logger.Log(LogLevel.Trace, null, message, arg1);
	public static void LogTrace<T, T1, T2>(this ILogger<T> logger, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Trace, null, message, arg1, arg2);

	// --- Debug ---
	public static void LogDebug<T>(this ILogger<T> logger, string message) where T : class => logger.Log(LogLevel.Debug, null, message);
	public static void LogDebug<T, T1>(this ILogger<T> logger, string message, T1 arg1) where T : class => logger.Log(LogLevel.Debug, null, message, arg1);
	public static void LogDebug<T, T1, T2>(this ILogger<T> logger, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Debug, null, message, arg1, arg2);

	// --- Information ---
	public static void LogInformation<T>(this ILogger<T> logger, string message) where T : class => logger.Log(LogLevel.Information, null, message);
	public static void LogInformation<T, T1>(this ILogger<T> logger, string message, T1 arg1) where T : class => logger.Log(LogLevel.Information, null, message, arg1);
	public static void LogInformation<T, T1, T2>(this ILogger<T> logger, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Information, null, message, arg1, arg2);

	// --- Warning ---
	public static void LogWarning<T>(this ILogger<T> logger, string message) where T : class => logger.Log(LogLevel.Warning, null, message);
	public static void LogWarning<T, T1>(this ILogger<T> logger, string message, T1 arg1) where T : class => logger.Log(LogLevel.Warning, null, message, arg1);
	public static void LogWarning<T, T1, T2>(this ILogger<T> logger, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Warning, null, message, arg1, arg2);

	// --- Error ---
	public static void LogError<T>(this ILogger<T> logger, Exception? ex, string message) where T : class => logger.Log(LogLevel.Error, ex, message);
	public static void LogError<T, T1>(this ILogger<T> logger, Exception? ex, string message, T1 arg1) where T : class => logger.Log(LogLevel.Error, ex, message, arg1);
	public static void LogError<T, T1, T2>(this ILogger<T> logger, Exception? ex, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Error, ex, message, arg1, arg2);

	// --- Critical ---
	public static void LogCritical<T>(this ILogger<T> logger, Exception? ex, string message) where T : class => logger.Log(LogLevel.Critical, ex, message);
	public static void LogCritical<T, T1>(this ILogger<T> logger, Exception? ex, string message, T1 arg1) where T : class => logger.Log(LogLevel.Critical, ex, message, arg1);
	public static void LogCritical<T, T1, T2>(this ILogger<T> logger, Exception? ex, string message, T1 arg1, T2 arg2) where T : class => logger.Log(LogLevel.Critical, ex, message, arg1, arg2);


	// --- Trace ---
	public static void LogTrace(this ILogger logger, Type type, string message)
		=> logger.Log(type, LogLevel.Trace, null, message);
	public static void LogTrace<T1>(this ILogger logger, Type type, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Trace, null, message, arg1);
	public static void LogTrace<T1, T2>(this ILogger logger, Type type, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Trace, null, message, arg1, arg2);

	// --- Debug ---
	public static void LogDebug(this ILogger logger, Type type, string message)
		=> logger.Log(type, LogLevel.Debug, null, message);
	public static void LogDebug<T1>(this ILogger logger, Type type, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Debug, null, message, arg1);
	public static void LogDebug<T1, T2>(this ILogger logger, Type type, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Debug, null, message, arg1, arg2);

	// --- Information ---
	public static void LogInformation(this ILogger logger, Type type, string message)
		=> logger.Log(type, LogLevel.Information, null, message);
	public static void LogInformation<T1>(this ILogger logger, Type type, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Information, null, message, arg1);
	public static void LogInformation<T1, T2>(this ILogger logger, Type type, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Information, null, message, arg1, arg2);

	// --- Warning ---
	public static void LogWarning(this ILogger logger, Type type, string message)
		=> logger.Log(type, LogLevel.Warning, null, message);
	public static void LogWarning<T1>(this ILogger logger, Type type, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Warning, null, message, arg1);
	public static void LogWarning<T1, T2>(this ILogger logger, Type type, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Warning, null, message, arg1, arg2);

	// --- Error ---
	public static void LogError(this ILogger logger, Type type, Exception? ex, string message)
		=> logger.Log(type, LogLevel.Error, ex, message);
	public static void LogError<T1>(this ILogger logger, Type type, Exception? ex, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Error, ex, message, arg1);
	public static void LogError<T1, T2>(this ILogger logger, Type type, Exception? ex, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Error, ex, message, arg1, arg2);

	// --- Critical ---
	public static void LogCritical(this ILogger logger, Type type, Exception? ex, string message)
		=> logger.Log(type, LogLevel.Critical, ex, message);
	public static void LogCritical<T1>(this ILogger logger, Type type, Exception? ex, string message, T1 arg1)
		=> logger.Log(type, LogLevel.Critical, ex, message, arg1);
	public static void LogCritical<T1, T2>(this ILogger logger, Type type, Exception? ex, string message, T1 arg1, T2 arg2)
		=> logger.Log(type, LogLevel.Critical, ex, message, arg1, arg2);
}
