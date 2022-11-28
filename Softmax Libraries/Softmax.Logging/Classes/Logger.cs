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
	public abstract class Logger<T> : object, ILogger<T> where T : class
	{
		#region Constructor
		protected Logger(IHttpContextAccessor httpContextAccessor = null) : base()
		{
			HttpContextAccessor = httpContextAccessor;
		}
		#endregion /Constructor

		public IHttpContextAccessor HttpContextAccessor { get; }

		#region GetExceptions
		protected virtual List<object> GetExceptions(Exception exception)
		{
			if (exception == null)
			{
				return null;
			}

			var exceptions = new List<object>();

			Exception currentException = exception;

			int index = 1;

			while (currentException != null)
			{
				exceptions.Add(new
				{
					Message = $"{currentException.Message} - (Message Level: {index})",
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

		#region Log
		protected bool Log(string logLevel,
			string message,
			Exception exception = null,
			List<object> parameters = null,
			Type classType = null,
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

				if ((HttpContextAccessor != null) &&
					(HttpContextAccessor.HttpContext != null) &&
					(HttpContextAccessor.HttpContext.Connection != null) &&
					(HttpContextAccessor.HttpContext.Connection.RemoteIpAddress != null))
				{
					logModel.RemoteIP =
						HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
				}

				if ((HttpContextAccessor != null) &&
					(HttpContextAccessor.HttpContext != null) &&
					(HttpContextAccessor.HttpContext.Connection != null) &&
					(HttpContextAccessor.HttpContext.Connection.LocalIpAddress != null))
				{
					logModel.LocalIP =
						HttpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString();
				}

				if ((HttpContextAccessor != null) &&
					(HttpContextAccessor.HttpContext != null) &&
					(HttpContextAccessor.HttpContext.Connection != null))
				{
					logModel.LocalPort =
						HttpContextAccessor.HttpContext.Connection.LocalPort.ToString();
				}

				if ((HttpContextAccessor != null) &&
					(HttpContextAccessor.HttpContext != null) &&
					(HttpContextAccessor.HttpContext.User != null) &&
					(HttpContextAccessor.HttpContext.User.Identity != null))
				{
					logModel.Username =
						HttpContextAccessor.HttpContext.User.Identity.Name;
				}

				if ((HttpContextAccessor != null) &&
					(HttpContextAccessor.HttpContext != null) &&
					(HttpContextAccessor.HttpContext.Request != null))
				{
					logModel.RequestPath =
						HttpContextAccessor.HttpContext.Request.Path;

					logModel.HttpReferrer =
						HttpContextAccessor.HttpContext.Request.Headers["Referer"];
				}

				logModel.ApplicationName =
					classType?.Assembly.FullName.ToString() ??
						typeof(T).GetTypeInfo().Assembly.FullName.ToString();

				logModel.ClassName = classType?.Name ?? typeof(T).Name;

				if (!string.IsNullOrWhiteSpace(methodName))
					logModel.MethodName = methodName;

				logModel.Namespace = classType?.Namespace ?? typeof(T).Namespace;

				logModel.Message = message;

				logModel.Exceptions =
					GetExceptions(exception: exception);

				logModel.Parameters =
					GetParameters(parameters: parameters);

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

		protected abstract void LogByFavoriteLibrary(LogModel logModel, Exception exception);

		#region LogTrace
		public virtual bool LogTrace
			(string message,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
						logLevel: "Trace",
						message: message,
						exception: null,
						classType: classType,
						parameters: parameters);

			return result;
		}
		#endregion /LogTrace

		#region LogDebug
		public virtual bool LogDebug
			(string message,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
					logLevel: "Debug",
					message: message,
					exception: null,
					classType: classType,
					parameters: parameters);

			return result;
		}
		#endregion /LogDebug

		#region LogInformation
		public virtual bool LogInformation
			(string message,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
					logLevel: "Information",
					message: message,
					exception: null,
					classType: classType,
					parameters: parameters);

			return result;
		}
		#endregion /LogInformation

		#region LogWarning
		public virtual bool LogWarning
			(string message,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
					logLevel: "Warning",
					message: message,
					exception: null,
					classType: classType,
					parameters: parameters);

			return result;
		}
		#endregion /LogWarning

		#region LogError
		public virtual bool LogError
			(Exception exception,
			string message = null,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (exception == null)
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
					logLevel: "Error",
					message: message,
					exception: exception,
					classType: classType,
					parameters: parameters);

			return result;
		}
		#endregion /LogError

		#region LogCritical
		public virtual bool LogCritical
			(Exception exception,
			string message = null,
			[CallerMemberName] string methodName = null,
			Type classType = null,
			List<object> parameters = null)
		{
			if (exception == null)
			{
				return false;
			}

			bool result =
					Log(methodName: methodName,
					logLevel: "Critical",
					message: message,
					exception: exception,
					classType: classType,
					parameters: parameters);

			return result;
		}
		#endregion /LogCritical
	}
}
