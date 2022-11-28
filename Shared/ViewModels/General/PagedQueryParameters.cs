namespace ViewModels.General;

public abstract class PagedQueryParameters
{
	private int _pageSize = 10;
	private readonly int _maxPageSize = 15;

	public int PageNumber { get; set; } = 1;
	public int PageSize
	{
		get
		{
			return _pageSize;
		}
		set
		{
			_pageSize = value > _maxPageSize ? _maxPageSize : value;
		}
	}
}
