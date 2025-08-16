using Microsoft.AspNetCore.Http;
using NLog;

namespace Dtat.Logging.NLogAdapter;

public class NLogAdapter<T> : Logger<T> where T : class
{
	public NLog.Logger Logger { get; set; }

	public NLogAdapter(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
	{
		Logger =
			LogManager.GetLogger(name: typeof(T).ToString());

		IsTraceEnabled = Logger.IsTraceEnabled;
		IsDebugEnabled = Logger.IsDebugEnabled;
		IsInformationEnabled = Logger.IsInfoEnabled;
		IsErrorEnabled = Logger.IsErrorEnabled;
		IsCriticalEnabled = Logger.IsFatalEnabled;
		IsWarningEnabled = Logger.IsWarnEnabled;
	}

	protected override void LogByFavoriteLibrary(LogModel logModel, System.Exception? exception)
	{
		// 1. Map our log level to NLog's LogLevel
		var nLogLevel = ToNLogLevel(logModel.LogLevel);

		// 2. Create a LogEventInfo object
		var logEvent = 
			new LogEventInfo(nLogLevel, "NLog", null, logModel.Message, parameters: logModel.Parameters);

		if (logModel.Parameters?.Length == 1 && logModel.Message?.Contains('{') == false)
		{
			var parameterObject = logModel.Parameters[0];
			if (parameterObject != null)
			{
				var type = parameterObject.GetType();

				if (!ObjectDestructurer.IsSimpleType(type))
				{
					var properties = ObjectDestructurer.Destructure(parameterObject);
					foreach (var property in properties)
					{
						logEvent.Properties[property.Key] = property.Value;
					}
				}
				else
				{
					// If it's a simple type, just add it as a single property.
					logEvent.Properties["ParameterValue"] = parameterObject;
				}
			}
		}

		// 4. Add all custom properties from our LogModel to NLog's event properties
		// This is the key to structured logging!
		logEvent.Properties["ApplicationName"] = logModel.ApplicationName;
		logEvent.Properties["Namespace"] = logModel.Namespace;
		logEvent.Properties["ClassName"] = logModel.ClassName;
		logEvent.Properties["MethodName"] = logModel.MethodName;
		logEvent.Properties["RemoteIP"] = logModel.RemoteIP;
		logEvent.Properties["LocalIP"] = logModel.LocalIP;
		logEvent.Properties["LocalPort"] = logModel.LocalPort;
		logEvent.Properties["AppUserName"] = logModel.UserName;
		logEvent.Properties["RequestPath"] = logModel.RequestPath;
		logEvent.Properties["HttpReferrer"] = logModel.HttpReferrer;
		logEvent.Properties["Exceptions"] = logModel.Exceptions;
		// Add any other custom properties you have...

		// 5. Log the rich event object
		Logger.Log(logEvent);
	}

	private static NLog.LogLevel ToNLogLevel(Dtat.Logging.LogLevel logLevel) => logLevel switch
	{
		Dtat.Logging.LogLevel.Trace => NLog.LogLevel.Trace,
		Dtat.Logging.LogLevel.Debug => NLog.LogLevel.Debug,
		Dtat.Logging.LogLevel.Information => NLog.LogLevel.Info,
		Dtat.Logging.LogLevel.Warning => NLog.LogLevel.Warn,
		Dtat.Logging.LogLevel.Error => NLog.LogLevel.Error,
		Dtat.Logging.LogLevel.Critical => NLog.LogLevel.Fatal,
		_ => NLog.LogLevel.Off, // Default case for safety
	};
}

public class NLogAdapter : Logger
{
	public NLog.Logger Logger { get; set; }

	public NLogAdapter(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
	{
		Logger =
			LogManager.GetLogger(name: nameof(NLogAdapter));

		IsTraceEnabled = Logger.IsTraceEnabled;
		IsDebugEnabled = Logger.IsDebugEnabled;
		IsInformationEnabled = Logger.IsInfoEnabled;
		IsErrorEnabled = Logger.IsErrorEnabled;
		IsCriticalEnabled = Logger.IsFatalEnabled;
		IsWarningEnabled = Logger.IsWarnEnabled;
	}

	protected override void LogByFavoriteLibrary(LogModel logModel, System.Exception? exception)
	{
		// 1. Map our log level to NLog's LogLevel
		var nLogLevel = ToNLogLevel(logModel.LogLevel);

		// 2. Create a LogEventInfo object
		var logEvent = 
			new LogEventInfo(nLogLevel, "NLog", null, logModel.Message, parameters: logModel.Parameters);

		if (logModel.Parameters?.Length == 1 && logModel.Message?.Contains('{') == false)
		{
			var parameterObject = logModel.Parameters[0];
			if (parameterObject != null)
			{
				var type = parameterObject.GetType();

				if (!ObjectDestructurer.IsSimpleType(type))
				{
					var properties = ObjectDestructurer.Destructure(parameterObject);
					foreach (var property in properties)
					{
						logEvent.Properties[property.Key] = property.Value;
					}
				}
				else
				{
					// If it's a simple type, just add it as a single property.
					logEvent.Properties["ParameterValue"] = parameterObject;
				}
			}
		}

		// 4. Add all custom properties from our LogModel to NLog's event properties
		// This is the key to structured logging!
		logEvent.Properties["ApplicationName"] = logModel.ApplicationName;
		logEvent.Properties["Namespace"] = logModel.Namespace;
		logEvent.Properties["ClassName"] = logModel.ClassName;
		logEvent.Properties["MethodName"] = logModel.MethodName;
		logEvent.Properties["RemoteIP"] = logModel.RemoteIP;
		logEvent.Properties["LocalIP"] = logModel.LocalIP;
		logEvent.Properties["LocalPort"] = logModel.LocalPort;
		logEvent.Properties["AppUserName"] = logModel.UserName;
		logEvent.Properties["RequestPath"] = logModel.RequestPath;
		logEvent.Properties["HttpReferrer"] = logModel.HttpReferrer;
		logEvent.Properties["Exceptions"] = logModel.Exceptions;
		// Add any other custom properties you have...

		// 5. Log the rich event object
		Logger.Log(logEvent);
	}

	private static NLog.LogLevel ToNLogLevel(Dtat.Logging.LogLevel logLevel) => logLevel switch
	{
		Dtat.Logging.LogLevel.Trace => NLog.LogLevel.Trace,
		Dtat.Logging.LogLevel.Debug => NLog.LogLevel.Debug,
		Dtat.Logging.LogLevel.Information => NLog.LogLevel.Info,
		Dtat.Logging.LogLevel.Warning => NLog.LogLevel.Warn,
		Dtat.Logging.LogLevel.Error => NLog.LogLevel.Error,
		Dtat.Logging.LogLevel.Critical => NLog.LogLevel.Fatal,
		_ => NLog.LogLevel.Off, // Default case for safety
	};
}
