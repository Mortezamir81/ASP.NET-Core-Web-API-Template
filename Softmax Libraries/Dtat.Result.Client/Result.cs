namespace Dtat.Results.Client
{
	public class Result
	{
		public bool IsSuccess { get; set; }

		public int MessageCode { get; set; }

		public List<string> Messages { get; set; }
	}
}
