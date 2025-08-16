using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Dtat.Logging.NLogAdapter;

public class NLogAdapter<T> : ILogger<T> where T : class
{
	private readonly NLog.Logger _nlog;
	private readonly IHttpContextAccessor? _httpContextAccessor;
	private readonly ICallerInfoResolver _callerInfoResolver;

	public NLogAdapter(IHttpContextAccessor? httpContextAccessor = null, ICallerInfoResolver? callerInfoResolver = null)
	{
		_nlog = LogManager.GetLogger(typeof(T).FullName);
		_httpContextAccessor = httpContextAccessor;
		_callerInfoResolver = callerInfoResolver ?? new EmptyStackTraceResolver();
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return _nlog.IsEnabled(ToNLogLevel(logLevel));
	}

	// Overload for logs without parameters
	public void Log(LogLevel logLevel, Exception? exception, string? message)
	{
		if (!IsEnabled(logLevel))
			return; // Prevents any object creation if disabled

		var nLogLevel = ToNLogLevel(logLevel);
		var logEvent = new LogEventInfo(nLogLevel, _nlog.Name, message);

		LogEventInternal(logEvent, exception);
	}

	// Overload for logs with a params array of arguments
	public void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
	{
		if (!IsEnabled(logLevel))
			return; // Prevents array allocation if disabled

		var nLogLevel = ToNLogLevel(logLevel);
		var logEvent = new LogEventInfo(nLogLevel, _nlog.Name, null, message, args);

		// Special handling for a single object parameter to destructure it
		if (args?.Length == 1 && message?.Contains('{') == false)
		{
			AddParameterProperties(logEvent, args[0]);
		}

		LogEventInternal(logEvent, exception);
	}

	// Generic overload for a single parameter to avoid array allocation
	public void Log<T1>(LogLevel logLevel, Exception? exception, string? message, T1 parameter)
	{
		if (!IsEnabled(logLevel))
			return; // The key performance benefit is here

		var nLogLevel = ToNLogLevel(logLevel);
		var parameters = new object?[] { parameter };
		var logEvent = new LogEventInfo(nLogLevel, _nlog.Name, null, message, parameters);

		// This overload is for structured logging; destructure the object into properties.
		AddParameterProperties(logEvent, parameter);

		LogEventInternal(logEvent, exception);
	}

	// Generic overload for two parameters to avoid array allocation
	public void Log<T1, T2>(LogLevel logLevel, Exception? exception, string? message, T1 parameter1, T2 parameter2)
	{
		if (!IsEnabled(logLevel))
			return; // The key performance benefit is here

		var nLogLevel = ToNLogLevel(logLevel);

		// For multiple parameters, we assume they are for message templates.
		// Allocation happens here, but ONLY if the log level is enabled.
		var parameters = new object?[] { parameter1, parameter2 };
		var logEvent = new LogEventInfo(nLogLevel, _nlog.Name, null, message, parameters);

		LogEventInternal(logEvent, exception);
	}

	// This is the new central method that processes a pre-created LogEventInfo
	private void LogEventInternal(LogEventInfo logEvent, Exception? exception)
	{
		var callerInfo = _callerInfoResolver.Resolve(skipFrames: 5); // Adjusted skipFrames
		if (callerInfo != null)
		{
			logEvent.Properties["MethodName"] = callerInfo.MethodName;
		}

		logEvent.Properties["ApplicationName"] = typeof(T).Assembly?.FullName;
		logEvent.Properties["Namespace"] = typeof(T).Namespace;
		logEvent.Properties["ClassName"] = typeof(T).Name;

		var context = _httpContextAccessor?.HttpContext;
		if (context != null)
		{
			logEvent.Properties["RequestPath"] = context.Request.Path.Value;
			logEvent.Properties["RemoteIP"] = context.Connection.RemoteIpAddress?.ToString();
			logEvent.Properties["AppUserName"] = context.User?.Identity?.Name;
			logEvent.Properties["HttpReferrer"] = context.Request?.Headers?.Referer.ToString();
		}

		logEvent.Properties["Exceptions"] = GetExceptions(exception);

		_nlog.Log(logEvent);
	}

	// Helper method to destructure a single object parameter
	private static void AddParameterProperties(LogEventInfo logEvent, object? parameter)
	{
		if (parameter == null)
			return;

		var type = parameter.GetType();
		if (!ObjectDestructurer.IsSimpleType(type))
		{
			var properties = ObjectDestructurer.Destructure(parameter);
			foreach (var property in properties)
			{
				logEvent.Properties[property.Key] = property.Value;
			}
		}
		else
		{
			logEvent.Properties["ParameterValue"] = parameter;
		}
	}

	private static List<object>? GetExceptions(Exception? exception)
	{
		if (exception == null)
			return null;

		var exceptions = new List<object>();
		var currentException = exception;
		int index = 1;

		while (currentException != null)
		{
			exceptions.Add(new
			{
				Message = $"{currentException.Message} - (Message Level: {index})",
				StackTrace = currentException.StackTrace
			});
			currentException = currentException.InnerException;
			index++;
		}
		return exceptions;
	}

	private static NLog.LogLevel ToNLogLevel(LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => NLog.LogLevel.Trace,
		LogLevel.Debug => NLog.LogLevel.Debug,
		LogLevel.Information => NLog.LogLevel.Info,
		LogLevel.Warning => NLog.LogLevel.Warn,
		LogLevel.Error => NLog.LogLevel.Error,
		LogLevel.Critical => NLog.LogLevel.Fatal,
		_ => NLog.LogLevel.Off,
	};
}


public class NLogAdapter : ILogger // Directly implements the interface
{
	private readonly IHttpContextAccessor? _httpContextAccessor;
	private readonly ICallerInfoResolver _callerInfoResolver;
	private readonly ConcurrentDictionary<Type, NLog.Logger> _loggers = new();
	private static readonly NLog.Logger _defaultLogger = LogManager.GetCurrentClassLogger();

	public NLogAdapter(IHttpContextAccessor? httpContextAccessor = null, ICallerInfoResolver? callerInfoResolver = null)
	{
		_httpContextAccessor = httpContextAccessor;
		_callerInfoResolver = callerInfoResolver ?? new EmptyStackTraceResolver();
	}

	// Helper method to get or create a logger for a specific type
	private NLog.Logger GetLogger(Type type)
	{
		return _loggers.GetOrAdd(type, t => LogManager.GetLogger(t.FullName));
	}

	// The interface requires this method without a Type context.
	// We check against a default logger instance.
	public bool IsEnabled(LogLevel logLevel)
	{
		return _defaultLogger.IsEnabled(ToNLogLevel(logLevel));
	}

	public void Log(Type type, LogLevel logLevel, Exception? exception, string? message)
	{
		var logger = GetLogger(type);
		var nLogLevel = ToNLogLevel(logLevel);
		if (!logger.IsEnabled(nLogLevel))
			return;

		var logEvent = new LogEventInfo(nLogLevel, logger.Name, message);
		LogEventInternal(logger, logEvent, type, exception);
	}

	public void Log(Type type, LogLevel logLevel, Exception? exception, string? message, params object?[] args)
	{
		var logger = GetLogger(type);
		var nLogLevel = ToNLogLevel(logLevel);
		if (!logger.IsEnabled(nLogLevel))
			return;

		var logEvent = new LogEventInfo(nLogLevel, logger.Name, null, message, args);
		if (args?.Length == 1 && message?.Contains('{') == false)
		{
			AddParameterProperties(logEvent, args[0]);
		}
		LogEventInternal(logger, logEvent, type, exception);
	}

	public void Log<T1>(Type type, LogLevel logLevel, Exception? exception, string? message, T1 parameter)
	{
		var logger = GetLogger(type);
		var nLogLevel = ToNLogLevel(logLevel);
		if (!logger.IsEnabled(nLogLevel))
			return;

		var logEvent = new LogEventInfo(nLogLevel, logger.Name, message);
		AddParameterProperties(logEvent, parameter);
		LogEventInternal(logger, logEvent, type, exception);
	}

	public void Log<T1, T2>(Type type, LogLevel logLevel, Exception? exception, string? message, T1 parameter1, T2 parameter2)
	{
		var logger = GetLogger(type);
		var nLogLevel = ToNLogLevel(logLevel);
		if (!logger.IsEnabled(nLogLevel))
			return;

		var parameters = new object?[] { parameter1, parameter2 };
		var logEvent = new LogEventInfo(nLogLevel, logger.Name, null, message, parameters);
		LogEventInternal(logger, logEvent, type, exception);
	}

	private void LogEventInternal(NLog.Logger logger, LogEventInfo logEvent, Type type, Exception? exception)
	{
		// Note: skipFrames might need adjustment based on your call stack
		var callerInfo = _callerInfoResolver.Resolve(skipFrames: 5);
		if (callerInfo != null)
		{
			logEvent.Properties["MethodName"] = callerInfo.MethodName;
		}

		logEvent.Properties["ApplicationName"] = type.Assembly?.FullName;
		logEvent.Properties["Namespace"] = type.Namespace;
		logEvent.Properties["ClassName"] = type.Name;

		var context = _httpContextAccessor?.HttpContext;
		if (context != null)
		{
			logEvent.Properties["RequestPath"] = context.Request.Path.Value;
			logEvent.Properties["RemoteIP"] = context.Connection.RemoteIpAddress?.ToString();
			logEvent.Properties["AppUserName"] = context.User?.Identity?.Name;
			logEvent.Properties["HttpReferrer"] = context.Request?.Headers?.Referer.ToString();
		}

		logEvent.Properties["Exceptions"] = GetExceptions(exception);

		logger.Log(logEvent);
	}

	private static void AddParameterProperties(LogEventInfo logEvent, object? parameter)
	{
		if (parameter == null)
			return;
		var type = parameter.GetType();
		if (!ObjectDestructurer.IsSimpleType(type))
		{
			var properties = ObjectDestructurer.Destructure(parameter);
			foreach (var property in properties)
			{
				logEvent.Properties[property.Key] = property.Value;
			}
		}
		else
		{
			logEvent.Properties["ParameterValue"] = parameter;
		}
	}

	private List<object>? GetExceptions(Exception? exception)
	{
		if (exception == null)
			return null;
		var exceptions = new List<object>();
		var currentException = exception;
		int index = 1;
		while (currentException != null)
		{
			exceptions.Add(new
			{
				Message = $"{currentException.Message} - (Message Level: {index})",
				StackTrace = currentException.StackTrace
			});
			currentException = currentException.InnerException;
			index++;
		}
		return exceptions;
	}

	private static NLog.LogLevel ToNLogLevel(LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => NLog.LogLevel.Trace,
		LogLevel.Debug => NLog.LogLevel.Debug,
		LogLevel.Information => NLog.LogLevel.Info,
		LogLevel.Warning => NLog.LogLevel.Warn,
		LogLevel.Error => NLog.LogLevel.Error,
		LogLevel.Critical => NLog.LogLevel.Fatal,
		_ => NLog.LogLevel.Off,
	};
}
