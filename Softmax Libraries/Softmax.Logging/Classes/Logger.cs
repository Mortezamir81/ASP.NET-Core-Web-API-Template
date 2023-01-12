using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Dtat.Logging
{
	public abstract class LoggerBase
	{
		#region Constructor
		protected LoggerBase(IHttpContextAccessor httpContextAccessor = null) : base()
		{
			HttpContextAccessor = httpContextAccessor;
		}
		#endregion /Constructor

		#region Properties
		public IHttpContextAccessor HttpContextAccessor { get; }
		#endregion /Properties

		#region GetExceptions
		protected virtual List<object> GetExceptions(Exception exception)
		{
			if (exception == null)
				return null;

			var exceptions = new List<object>();

			Exception currentException = exception;

			int index = 1;

			while (currentException != null)
			{
				exceptions.Add(new
				{
					Message = $"{currentException.Message} - (Message Level: {index})",
					currentException.StackTrace,
				});

				currentException =
					currentException.InnerException;

				index++;
			}

			return exceptions;
		}
		#endregion /GetExceptions

		#region GetParameters
		protected virtual List<object> GetParameters(List<object> parameters)
		{
			List<object> returnValue = null;

			if (parameters != null && parameters.Count > 0)
			{
				returnValue = new List<object>();

				parameters.ForEach(current =>
				{
					if (current != null)
					{
						returnValue.Add(current);
					}
				});
			}

			return returnValue;
		}
		#endregion /GetParameters
	}

	public abstract class Logger<T> : LoggerBase, ILogger<T> where T : class
	{
		#region Constructor
		protected Logger(IHttpContextAccessor httpContextAccessor = null) : base(httpContextAccessor)
		{
		}
		#endregion /Constructor

		#region Log
		protected bool Log(string logLevel,
			string message,
			Exception exception = null,
			List<object> parameters = null,
			string methodName = null)
		{
			try
			{
				// **************************************************
				string currentCultureName =
					Thread.CurrentThread.CurrentCulture.Name;

				var newCultureInfo =
					new CultureInfo(name: "en-US");

				var currentCultureInfo =
					new CultureInfo(currentCultureName);

				Thread.CurrentThread.CurrentCulture = newCultureInfo;
				// **************************************************

				var logModel = new LogModel
				{
					LogLevel = logLevel,
				};

				var connection =
					HttpContextAccessor?.HttpContext?.Connection;

				logModel.RemoteIP =
					connection?.RemoteIpAddress?.ToString();
				
				logModel.LocalIP =
					connection?.LocalIpAddress?.ToString();
				
				logModel.LocalPort =
					connection?.LocalPort.ToString();
				
				logModel.Username =
					HttpContextAccessor?.HttpContext?.User?.Identity?.Name;

				var httpRequest =
					HttpContextAccessor?.HttpContext?.Request;

				logModel.RequestPath = httpRequest?.Path;

				logModel.HttpReferrer = httpRequest?.Headers["Referer"];
				
				logModel.ApplicationName =
						typeof(T).GetTypeInfo().Assembly.FullName.ToString();

				logModel.ClassName = typeof(T).Name;

				if (!string.IsNullOrWhiteSpace(methodName))
					logModel.MethodName = methodName;

				logModel.Namespace = typeof(T).Namespace;

				logModel.Message = message;

				logModel.Exceptions =
					GetExceptions(exception: exception);

				logModel.Parameters = parameters;

				LogByFavoriteLibrary(logModel: logModel, exception: exception);

				// **************************************************
				Thread.CurrentThread.CurrentCulture = currentCultureInfo;
				// **************************************************


				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion /Log

		#region LogTrace
		public virtual bool LogTrace
			(string message,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Trace),
					message: message,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogTrace

		#region LogDebug
		public virtual bool LogDebug
			(string message,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Debug),
					message: message,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogDebug

		#region LogInformation
		public virtual bool LogInformation
			(string message,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Information),
					message: message,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogInformation

		#region LogWarning
		public virtual bool LogWarning
			(string message,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Warning),
					message: message,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogWarning

		#region LogError
		public virtual bool LogError
			(Exception exception,
			string message = null,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (exception == null)
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Error),
					message: message,
					exception: exception,
					parameters: parameters);

			return result;
		}
		#endregion /LogError

		#region LogCritical
		public virtual bool LogCritical
			(Exception exception,
			string message = null,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (exception == null)
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Critical),
					message: message,
					exception: exception,
					parameters: parameters);

			return result;
		}
		#endregion /LogCritical

		protected abstract void LogByFavoriteLibrary(LogModel logModel, Exception exception);
	}

	public abstract class Logger : LoggerBase, ILogger
	{
		#region Constructor
		protected Logger(IHttpContextAccessor httpContextAccessor = null) : base(httpContextAccessor)
		{
		}
		#endregion /Constructor

		#region Log
		protected bool Log(string logLevel,
			string message,
			Type classType,
			Exception exception = null,
			List<object> parameters = null,
			string methodName = null)
		{
			try
			{
				// **************************************************
				string currentCultureName =
					Thread.CurrentThread.CurrentCulture.Name;

				var newCultureInfo =
					new CultureInfo(name: "en-US");

				var currentCultureInfo =
					new CultureInfo(currentCultureName);

				Thread.CurrentThread.CurrentCulture = newCultureInfo;
				// **************************************************

				var logModel = new LogModel
				{
					LogLevel = logLevel,
				};

				var connection =
					HttpContextAccessor?.HttpContext?.Connection;

				logModel.RemoteIP =
					connection?.RemoteIpAddress?.ToString();

				logModel.LocalIP =
					connection?.LocalIpAddress?.ToString();

				logModel.LocalPort =
					connection?.LocalPort.ToString();

				logModel.Username =
					HttpContextAccessor?.HttpContext?.User?.Identity?.Name;

				var httpRequest =
					HttpContextAccessor?.HttpContext?.Request;

				logModel.RequestPath = httpRequest?.Path;

				logModel.HttpReferrer = httpRequest?.Headers["Referer"];

				logModel.ApplicationName =
					classType?.Assembly.FullName.ToString();

				logModel.ClassName = classType?.Name;

				if (!string.IsNullOrWhiteSpace(methodName))
					logModel.MethodName = methodName;

				logModel.Namespace = classType?.Namespace;

				logModel.Message = message;

				logModel.Exceptions =
					GetExceptions(exception: exception);

				logModel.Parameters = parameters;

				LogByFavoriteLibrary(logModel: logModel, name: classType?.Name, exception: exception);

				// **************************************************
				Thread.CurrentThread.CurrentCulture = currentCultureInfo;
				// **************************************************


				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion /Log

		#region LogTrace
		public virtual bool LogTrace
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Trace),
					classType: classType,
					message: message,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogTrace

		#region LogDebug
		public virtual bool LogDebug
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Debug),
					message: message,
					classType: classType,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogDebug

		#region LogInformation
		public virtual bool LogInformation
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Information),
					message: message,
					classType: classType,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogInformation

		#region LogWarning
		public virtual bool LogWarning
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Warning),
					message: message,
					classType: classType,
					exception: null,
					parameters: parameters);

			return result;
		}
		#endregion /LogWarning

		#region LogError
		public virtual bool LogError
			(Exception exception,
			Type classType,
			string message = null,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (exception == null)
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Error),
					message: message,
					classType: classType,
					exception: exception,
					parameters: parameters);

			return result;
		}
		#endregion /LogError

		#region LogCritical
		public virtual bool LogCritical
			(Exception exception,
			Type classType,
			string message = null,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null)
		{
			if (exception == null)
				return false;

			bool result =
				Log(methodName: methodName,
					logLevel: nameof(LogLevel.Critical),
					message: message,
					classType: classType,
					exception: exception,
					parameters: parameters);

			return result;
		}
		#endregion /LogCritical

		protected abstract void LogByFavoriteLibrary(LogModel logModel, string name, Exception exception);
	}
}
