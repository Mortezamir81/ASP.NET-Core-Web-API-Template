using Microsoft.AspNetCore.Http;
using NLog;

namespace Dtat.Logging.NLog
{
	public class NLogAdapter<T> : Logger<T> where T : class
	{
		public NLogAdapter(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
		{
		}

		protected override void LogByFavoriteLibrary(LogModel log, System.Exception exception)
		{
			string loggerMessage = log.ToString();

			 var logger =
				LogManager.GetLogger(name: typeof(T).ToString());

			switch (log.LogLevel)
			{
				case "Trace":
				{
					logger.Trace
						(exception, message: loggerMessage);

					break;
				}

				case "Debug":
				{
					logger.Debug
						(exception, message: loggerMessage);

					break;
				}

				case "Information":
				{
					logger.Info
						(exception, message: loggerMessage);

					break;
				}

				case "Warning":
				{
					logger.Warn
						(exception, message: loggerMessage);

					break;
				}

				case "Error":
				{
					logger.Error
						(exception, message: loggerMessage);

					break;
				}

				case "Critical":
				{
					logger.Fatal
						(exception, message: loggerMessage);

					break;
				}
			}
		}
	}

	public class NLogAdapter : Logger
	{
		public NLogAdapter(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
		{
		}

		protected override void LogByFavoriteLibrary(LogModel log, string name, System.Exception exception)
		{
			string loggerMessage = log.ToString();

			var logger =
				LogManager.GetLogger(name: name);

			switch (log.LogLevel)
			{
				case "Trace":
					{
						logger.Trace
							(exception, message: loggerMessage);

						break;
					}

				case "Debug":
					{
						logger.Debug
							(exception, message: loggerMessage);

						break;
					}

				case "Information":
					{
						logger.Info
							(exception, message: loggerMessage);

						break;
					}

				case "Warning":
					{
						logger.Warn
							(exception, message: loggerMessage);

						break;
					}

				case "Error":
					{
						logger.Error
							(exception, message: loggerMessage);

						break;
					}

				case "Critical":
					{
						logger.Fatal
							(exception, message: loggerMessage);

						break;
					}
			}
		}
	}
}
