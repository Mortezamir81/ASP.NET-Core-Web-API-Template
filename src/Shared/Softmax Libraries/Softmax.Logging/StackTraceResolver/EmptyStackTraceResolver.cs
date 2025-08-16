namespace Dtat.Logging;

public class EmptyStackTraceResolver : ICallerInfoResolver
{
	public CallerInfo? Resolve(int skipFrames = 0)
	{
		return null;
	}
}
