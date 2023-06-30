using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dtat.Logging
{
	public interface ILogger<T> where T : class
	{
		bool LogTrace
			(string message,
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);

		bool LogDebug
			(string message,
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);

		bool LogInformation
			(string message, 
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);

		bool LogWarning
			(string message,
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);

		bool LogError
			(Exception exception,
			string message = null,
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);

		bool LogCritical
			(Exception exception,
			string message = null,
				[CallerMemberName] string methodName = null,
				List<object> parameters = null);
	}

	public interface ILogger
	{
		bool LogTrace
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null);

		bool LogDebug
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null);

		bool LogInformation
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null);

		bool LogWarning
			(string message,
			Type classType,
			[CallerMemberName] string methodName = null,
			List<object> parameters = null);

		bool LogError
			(Exception exception,
			Type classType,
			string message = null, [CallerMemberName] string methodName = null,
			List<object> parameters = null);

		bool LogCritical
			(Exception exception,
			Type classType,
			string message = null, [CallerMemberName] string methodName = null,
			List<object> parameters = null);
	}
}
