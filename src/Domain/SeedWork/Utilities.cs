namespace Domain.SeedWork;

public class Utilities
{
	public static DateTime DateTimeNow
	{
		get
		{
			return DateTime.UtcNow;
		}
	}

	public static DateTimeOffset DateTimeOffsetNow
	{
		get
		{
			return DateTimeOffset.UtcNow;
		}
	}
}
