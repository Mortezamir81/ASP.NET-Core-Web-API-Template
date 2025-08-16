using Microsoft.Extensions.ObjectPool;

namespace Dtat.Logging;

public class LogModelPooledObjectPolicy : IPooledObjectPolicy<LogModel>
{
	// This method is called when the pool needs to create a new LogModel.
	public LogModel Create()
	{
		// We create it with a dummy log level, it will be reset later.
		return new LogModel(LogLevel.Information);
	}

	// This method is called when a LogModel is returned to the pool.
	public bool Return(LogModel obj)
	{
		// The Reset call is no longer needed here.
		// The LoggerBase will handle resetting the object with the correct log level
		// immediately after getting it from the pool.
		return true;
	}
}