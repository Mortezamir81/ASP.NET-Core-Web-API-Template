using Microsoft.AspNetCore.Http;
using NLog;

namespace Dtat.Logging.NLogAdapter
{
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

		protected override void LogByFavoriteLibrary(LogModel log, System.Exception exception)
		{
			string loggerMessage = log.ToString();

			switch (log.LogLevel)
			{
				case "Trace":
				{
					Logger.Trace
						(exception, message: loggerMessage);

					break;
				}

				case "Debug":
				{
					Logger.Debug
						(exception, message: loggerMessage);

					break;
				}

				case "Information":
				{
					Logger.Info
						(exception, message: loggerMessage);

					break;
				}

				case "Warning":
				{
					Logger.Warn
						(exception, message: loggerMessage);

					break;
				}

				case "Error":
				{
					Logger.Error
						(exception, message: loggerMessage);

					break;
				}

				case "Critical":
				{
					Logger.Fatal
						(exception, message: loggerMessage);

					break;
				}
			}
		}
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

		protected override void LogByFavoriteLibrary(LogModel log, System.Exception exception)
		{
			string loggerMessage = log.ToString();

			switch (log.LogLevel)
			{
				case "Trace":
					{
						Logger.Trace
							(exception, message: loggerMessage);

						break;
					}

				case "Debug":
					{
						Logger.Debug
							(exception, message: loggerMessage);

						break;
					}

				case "Information":
					{
						Logger.Info
							(exception, message: loggerMessage);

						break;
					}

				case "Warning":
					{
						Logger.Warn
							(exception, message: loggerMessage);

						break;
					}

				case "Error":
					{
						Logger.Error
							(exception, message: loggerMessage);

						break;
					}

				case "Critical":
					{
						Logger.Fatal
							(exception, message: loggerMessage);

						break;
					}
			}
		}
	}
}
