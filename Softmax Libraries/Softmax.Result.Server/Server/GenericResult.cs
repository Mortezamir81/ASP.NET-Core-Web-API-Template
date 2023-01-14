using System;

namespace Dtat.Results.Server
{
	public class Result<T> : Result
	{
		public Result() : base()
		{
		}


		public T Value { get; set; }
	}
}
