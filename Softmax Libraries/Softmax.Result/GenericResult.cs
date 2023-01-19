using System;

namespace Dtat.Result
{
	public class Result<T> : Result
	{
		public Result() : base()
		{
		}


		public T Value { get; set; }
	}
}
