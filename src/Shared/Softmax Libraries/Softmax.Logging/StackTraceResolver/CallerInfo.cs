namespace Dtat.Logging;

/// <summary>
/// A record to hold the resolved caller information.
/// </summary>
public record CallerInfo
{
	public string? MethodName { get; init; }
	public string? FilePath { get; init; }
	public int LineNumber { get; init; }

	public static readonly CallerInfo Empty = new()
	{
		MethodName = "N/A",
		FilePath = "N/A",
		LineNumber = 0
	};

	public override string ToString() => $"{MethodName} ({FilePath}:{LineNumber})";
}
