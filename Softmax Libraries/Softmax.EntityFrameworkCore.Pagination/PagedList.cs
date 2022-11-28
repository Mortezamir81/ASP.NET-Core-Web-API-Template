using Microsoft.EntityFrameworkCore;

namespace Softmax.Data.EntityFrameworkCore
{
	public class PagedList<T> : List<T>
	{
		#region Constractor
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);

			AddRange(items);
		}
		#endregion /Constractor

		#region Properties
		public int CurrentPage { get; private set; }
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }

		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;
		#endregion /Properties

		#region Methods
		public async static Task<PagedList<T>>
		ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
		{
			var count = await source.CountAsync();

			var items =
				await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

			return new PagedList<T>(items, count, pageNumber, pageSize);
		}
		#endregion /Methods
	}
}