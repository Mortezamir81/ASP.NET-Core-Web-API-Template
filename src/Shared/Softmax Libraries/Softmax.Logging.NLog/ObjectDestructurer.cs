// Create a new file: ObjectDestructurer.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dtat.Logging.NLogAdapter;

internal static class ObjectDestructurer
{
	// A thread-safe cache for our compiled destructuring functions.
	private static readonly ConcurrentDictionary<Type, Func<object, IEnumerable<KeyValuePair<string, object?>>>> _cache = new();

	private static readonly HashSet<Type> _simpleTypes = new HashSet<Type>
	{
		typeof(string),
		typeof(decimal),
		typeof(DateTime),
		typeof(DateTimeOffset),
		typeof(TimeSpan),
		typeof(Guid),
		typeof(Uri), // Uri is often treated as a simple value
		typeof(DateOnly), // .NET 6+
		typeof(TimeOnly), // .NET 6+
		typeof(DBNull),
	};

	public static IEnumerable<KeyValuePair<string, object?>> Destructure(object obj)
	{
		var type = obj.GetType();

		// Get the destructuring function from the cache, or create it if it doesn't exist.
		var func = _cache.GetOrAdd(type, CreateDestructuringFunc);

		return func(obj);
	}

	private static Func<object, IEnumerable<KeyValuePair<string, object?>>> CreateDestructuringFunc(Type type)
	{
		// This is where the magic happens. We build an expression tree.
		var objParameter = Expression.Parameter(typeof(object), "obj");
		var typedObj = Expression.Convert(objParameter, type);

		var properties = type.GetProperties().Where(p => p.CanRead);

		var newExpressions = properties.Select(p =>
		{
			var key = Expression.Constant(p.Name);
			var value = Expression.Property(typedObj, p);
			var valueAsObject = Expression.Convert(value, typeof(object));

			// Create a new KeyValuePair<string, object?>(key, value)
			var ctor = typeof(KeyValuePair<string, object?>).GetConstructor(new[] { typeof(string), typeof(object) });
			return Expression.New(ctor!, key, valueAsObject);
		});

		// Create an array of KeyValuePair expressions
		var arrayExpression = Expression.NewArrayInit(typeof(KeyValuePair<string, object?>), newExpressions);

		// Compile the expression tree into a super-fast delegate and return it.
		var lambda = Expression.Lambda<Func<object, IEnumerable<KeyValuePair<string, object?>>>>(arrayExpression, objParameter);
		return lambda.Compile();
	}

	public static bool IsSimpleType(Type type)
	{
		// 1. Check for nullable value types and get the underlying type
		// For example, if type is `int?`, underlyingType will be `int`.
		var underlyingType = Nullable.GetUnderlyingType(type);

		if (underlyingType != null)
			type = underlyingType;

		// 2. Check for primitive types (int, bool, double, char, etc.), enums, and our predefined simple types.
		return type.IsPrimitive ||
			   type.IsEnum ||
			   _simpleTypes.Contains(type);
	}
}