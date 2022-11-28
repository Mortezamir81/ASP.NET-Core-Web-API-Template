namespace Dtat.Results.Client
{
	public class Result<T> : Result
	{
		public Result() : base()
		{
		}

		public T Value { get; set; }
		public Pagination Pagination { get; set; }
	}
}
