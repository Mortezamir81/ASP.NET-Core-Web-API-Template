using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtat.Logging;

public interface ICallerInfoResolver
{
	/// <summary>
	/// Resolves the caller information by walking the stack.
	/// </summary>
	/// <param name="skipFrames">The initial number of frames to skip.</param>
	/// <returns>CallerInfo or null if not found.</returns>
	CallerInfo? Resolve(int skipFrames = 0);
}
