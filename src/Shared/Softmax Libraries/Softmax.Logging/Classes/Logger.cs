using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Dtat.Logging;

public abstract class LoggerBase
{
	protected static readonly ThreadLocal<CultureInfo> _englishCulture = new(() => new CultureInfo("en-US"));
	private static readonly ObjectPool<LogModel> _logModelPool =
		new DefaultObjectPool<LogModel>(new LogModelPooledObjectPolicy());

	#region Constructor
	protected LoggerBase(
		IHttpContextAccessor? httpContextAccessor = null,
		ICallerInfoResolver? callerInfoResolver = null,
		StackTraceResolverOptions? stackTraceResolverOptions = null) : base()
	{
		HttpContextAccessor = httpContextAccessor;
		CallerInfoResolver = callerInfoResolver ?? new SmartStackTraceResolver(stackTraceResolverOptions ?? new StackTraceResolverOptions());
	}
	#endregion /Constructor

	#region Properties
	protected ICallerInfoResolver CallerInfoResolver { get; }
	public IHttpContextAccessor? HttpContextAccessor { get; }
	public bool IsTraceEnabled { get; set; } = false;
	public bool IsDebugEnabled { get; set; } = false;
	public bool IsInformationEnabled { get; set; } = false;
	public bool IsErrorEnabled { get; set; } = false;
	public bool IsCriticalEnabled { get; set; } = false;
	public bool IsWarningEnabled { get; set; } = false;
	#endregion /Properties

	#region GetExceptions
	protected virtual List<ExceptionModel>? GetExceptions(Exception? exception)
	{
		if (exception == null)
			return null;

		var exceptions = new List<ExceptionModel>();
		var currentException = exception;
		var index = 1;
		var messageBuilder = new System.Text.StringBuilder();

		while (currentException != null)
		{
			messageBuilder.Clear();
			messageBuilder.Append(currentException.Message);
			messageBuilder.Append(" - (Message Level: ");
			messageBuilder.Append(index);
			messageBuilder.Append(')');

			exceptions.Add(new ExceptionModel(messageBuilder.ToString())
			{
				StackTrace = currentException.StackTrace,
			});

			currentException = currentException.InnerException;
			index++;
		}

		return exceptions;
	}
	#endregion /GetExceptions

	#region Log
	protected void Log(
			LogLevel logLevel,
			Type classType,
			string? message,
			Exception? exception,
			string? methodName,
			object?[]? args)
	{
		LogModel? logModel = null;
		CultureInfo? originalCulture = null;

		try
		{
			if (!Thread.CurrentThread.CurrentCulture.Name.Equals("en-US", StringComparison.OrdinalIgnoreCase))
			{
				originalCulture = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = _englishCulture.Value!;
			}

			var context = HttpContextAccessor?.HttpContext;
			logModel = _logModelPool.Get();
			logModel.Reset(logLevel);

			logModel.Message = message; // Storing the template
			logModel.Parameters = args; // Storing the args array
			logModel.Namespace = classType.Namespace;
			logModel.ClassName = classType.Name;
			logModel.ApplicationName = classType.Assembly?.FullName;
			logModel.MethodName = methodName;
			logModel.Exceptions = GetExceptions(exception);

			if (context != null)
			{
				var connection = context.Connection;
				var httpRequest = context.Request;

				logModel.RemoteIP = connection?.RemoteIpAddress?.ToString();
				logModel.LocalIP = connection?.LocalIpAddress?.ToString();
				logModel.LocalPort = connection?.LocalPort.ToString();
				logModel.UserName = context.User?.Identity?.Name;
				logModel.RequestPath = httpRequest?.Path.Value;
				logModel.HttpReferrer = httpRequest?.Headers["Referer"].ToString();
			}

			LogByFavoriteLibrary(logModel, exception);
		}
		catch(Exception ex)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine($"[Dtat.Logging Internal Error]: {ex}");
#endif
		}
		finally
		{
			if (originalCulture != null)
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}

			if (logModel != null)
			{
				_logModelPool.Return(logModel);
			}
		}
	}
	#endregion

	protected abstract void LogByFavoriteLibrary(LogModel logModel, Exception? exception);
}

public abstract class Logger<T> : LoggerBase, ILogger<T> where T : class
{
	#region Constructor
	protected Logger(
			IHttpContextAccessor? httpContextAccessor = null,
			ICallerInfoResolver? callerInfoResolver = null,
			StackTraceResolverOptions? stackTraceResolverOptions = null) : base(httpContextAccessor, callerInfoResolver, stackTraceResolverOptions)
	{
	}
	#endregion /Constructor

	#region Log
	private void Log(
			LogLevel logLevel,
			Exception? exception,
			string message,
			params object?[]? args)
	{
		// Use the resolver to get caller info
		// The number 2 is passed to skip Log() and LogInformation() frames
		var callerInfo = CallerInfoResolver.Resolve(skipFrames: 2);

		base.Log(
			logLevel: logLevel,
			classType: typeof(T),
			message: message,
			exception: exception,
			methodName: callerInfo?.MethodName,
			args: args
		);
	}
	#endregion /Log

	public virtual void LogTrace(string message, params object?[] args)
	{
		if (!IsTraceEnabled)
			return;
		Log(LogLevel.Trace, null, message, args);
	}

	public virtual void LogDebug(string message, params object?[] args)
	{
		if (!IsDebugEnabled)
			return;
		Log(LogLevel.Debug, null, message, args);
	}

	public virtual void LogInformation(string message, params object?[] args)
	{
		if (!IsInformationEnabled)
			return;
		Log(LogLevel.Information, null, message, args);
	}

	public virtual void LogWarning(string message, params object?[] args)
	{
		if (!IsWarningEnabled)
			return;
		Log(LogLevel.Warning, null, message, args);
	}

	public virtual void LogError(Exception exception, string message, params object?[] args)
	{
		if (!IsErrorEnabled)
			return;
		Log(LogLevel.Error, exception, message, args);
	}

	public virtual void LogCritical(Exception exception, string message, params object?[] args)
	{
		if (!IsCriticalEnabled)
			return;
		Log(LogLevel.Critical, exception, message, args);
	}
}

public abstract class Logger : LoggerBase, ILogger
{
	#region Constructor
	protected Logger(
			IHttpContextAccessor? httpContextAccessor = null,
			ICallerInfoResolver? callerInfoResolver = null,
			StackTraceResolverOptions? stackTraceResolverOptions = null) : base(httpContextAccessor, callerInfoResolver, stackTraceResolverOptions)
	{
	}
	#endregion /Constructor

	#region Log
	private void Log(
			LogLevel logLevel,
			Type classType,
			Exception? exception,
			string message,
			params object?[]? args)
	{
		// Use the resolver to get caller info.
		// We pass skipFrames: 2 to bypass the public Log method (e.g., LogInformation)
		// and this private Log method itself.
		var callerInfo = CallerInfoResolver.Resolve(skipFrames: 2);

		base.Log(
			logLevel: logLevel,
			classType: classType,
			message: message,
			exception: exception,
			methodName: callerInfo?.MethodName,
			args: args
		);
	}
	#endregion /Log

	public virtual void LogTrace(Type classType, string message, params object?[] args)
	{
		if (!IsTraceEnabled)
			return;
		Log(LogLevel.Trace, classType, null, message, args);
	}

	public virtual void LogDebug(Type classType, string message, params object?[] args)
	{
		if (!IsDebugEnabled)
			return;
		Log(LogLevel.Debug, classType, null, message, args);
	}

	public virtual void LogInformation(Type classType, string message, params object?[] args)
	{
		if (!IsInformationEnabled)
			return;
		Log(LogLevel.Information, classType, null, message, args);
	}

	public virtual void LogWarning(Type classType, string message, params object?[] args)
	{
		if (!IsWarningEnabled)
			return;
		Log(LogLevel.Warning, classType, null, message, args);
	}

	public virtual void LogError(Exception exception, Type classType, string message, params object?[] args)
	{
		if (!IsErrorEnabled)
			return;
		Log(LogLevel.Error, classType, exception, message, args);
	}

	public virtual void LogCritical(Exception exception, Type classType, string message, params object?[] args)
	{
		if (!IsCriticalEnabled)
			return;
		Log(LogLevel.Critical, classType, exception, message, args);
	}
}
