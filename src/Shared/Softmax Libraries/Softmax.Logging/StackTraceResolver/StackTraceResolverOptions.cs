using System.Collections.Generic;

namespace Dtat.Logging;

/// <summary>
/// Options to configure the behavior of the stack trace resolver.
/// </summary>
public class StackTraceResolverOptions
{
	/// <summary>
	/// A set of namespace prefixes to ignore when resolving the call site.
	/// Default includes System, Microsoft, and common logging frameworks.
	/// </summary>
	public HashSet<string> IgnoredNamespaces { get; set; } = new()
	{
		"System",
		"Microsoft",
		"NLog",
		"Serilog",
		"Dtat.Logging" // Assuming this is our own logging framework
    };

	/// <summary>
	/// The maximum number of frames to walk up the stack.
	/// Prevents infinite loops and limits performance impact.
	/// </summary>
	public int MaxStackDepth { get; set; } = 20;
}