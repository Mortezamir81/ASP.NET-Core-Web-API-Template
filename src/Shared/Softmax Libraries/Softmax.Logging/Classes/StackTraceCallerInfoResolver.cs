namespace Dtat.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

public interface ICallerInfoResolver
{
	/// <summary>
	/// Resolves the caller information by walking the stack.
	/// </summary>
	/// <param name="skipFrames">The initial number of frames to skip.</param>
	/// <returns>CallerInfo or null if not found.</returns>
	CallerInfo? Resolve(int skipFrames = 0);
}

public class SmartStackTraceResolver : ICallerInfoResolver
{
	private readonly StackTraceResolverOptions _options;

	public SmartStackTraceResolver(StackTraceResolverOptions options)
	{
		_options = options;
		// Automatically ignore the resolver's own namespace.
		var resolverNamespace = typeof(SmartStackTraceResolver).Namespace;
		if (!string.IsNullOrEmpty(resolverNamespace))
		{
			_options.IgnoredNamespaces.Add(resolverNamespace);
		}
	}

	public CallerInfo? Resolve(int skipFrames = 0)
	{
		try
		{
			// Start with the initial skip count and add 1 to skip this Resolve method itself.
			for (int frameIndex = skipFrames + 1; frameIndex < _options.MaxStackDepth; frameIndex++)
			{
				var frame = new StackFrame(frameIndex, needFileInfo: true);
				MethodBase? method = frame.GetMethod();

				if (method == null)
					break; // Reached end of stack

				Type? declaringType = method.DeclaringType;
				if (declaringType == null)
					continue; // Skip global methods

				// Condition 1: Skip compiler-generated types (async state machines, etc.)
				if (declaringType.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
				{
					continue;
				}

				// Condition 2: Skip ignored namespaces
				string? ns = declaringType.Namespace ?? string.Empty;
				if (_options.IgnoredNamespaces.Any(ignoredNs => ns.StartsWith(ignoredNs, StringComparison.Ordinal)))
				{
					continue;
				}

				// Found the user code!
				return new CallerInfo
				{
					MethodName = method.Name,
				};
			}
		}
		catch
		{
			// If any exception occurs (e.g., security), return null safely.
			return null;
		}

		return null;
	}
}

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