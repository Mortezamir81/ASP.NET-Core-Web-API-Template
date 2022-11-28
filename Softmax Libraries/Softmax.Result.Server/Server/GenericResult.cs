using System;

namespace Dtat.Results.Server
{
	public class Result<T> : Result
	{
		public Result() : base()
		{
		}


		public T Value { get; set; }
		public Pagination Pagination { get; private set; }


		public void AddPagination(int currentPage, int totalPages, int pageSize, int totalCount)
		{
			if (currentPage <= 0)
			{
				throw new ArgumentException
					(message: $"Invalid Parameter: {nameof(currentPage)}");
			}

			if (totalPages <= 0)
			{
				throw new ArgumentException
					(message: $"Invalid Parameter: {nameof(totalPages)}");
			}

			if (pageSize <= 0)
			{
				throw new ArgumentException
					(message: $"Invalid Parameter: {nameof(pageSize)}");
			}

			Pagination = new Pagination
			{
				CurrentPage = currentPage,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}
	}
}
