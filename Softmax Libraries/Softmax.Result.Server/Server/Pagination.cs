namespace Dtat.Results.Server
{
	public class Pagination
	{
		public int CurrentPage { get; internal set; }
		public int TotalPages { get; internal set; }
		public int PageSize { get; internal set; }
		public int TotalCount { get; internal set; }

		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;
	}
}
