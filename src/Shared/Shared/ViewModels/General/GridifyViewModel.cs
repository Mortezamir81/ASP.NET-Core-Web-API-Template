namespace ViewModels;

public class GridifyViewModel
{
	private readonly int _maxPageSize = 15;

	public GridifyViewModel()
	{
		_pageSize = _maxPageSize;
	}

	public GridifyViewModel(int maxPageSize)
	{
		_maxPageSize = maxPageSize;
		_pageSize = _maxPageSize;
	}

	private int _pageSize;

	/// <summary>
	/// To learn how to write filter query see: https://alirezanet.github.io/Gridify/guide/filtering.html
	/// </summary>
	public string? Filter { get; set; }


	/// <summary>
	/// To learn how to write orderby query see: https://alirezanet.github.io/Gridify/guide/ordering.html
	/// </summary>
	public string? OrderBy { get; set; }

	public int Page { get; set; } = 1;

	public int PageSize
	{
		get => _pageSize;
		set
		{
			if (value > _maxPageSize)
			{
				_pageSize = _maxPageSize;

				return;
			}

			_pageSize = value;
		}
	}
}
