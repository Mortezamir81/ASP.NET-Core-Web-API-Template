namespace Domain.Common;

public class BaseEntity<TEntityKey>
{
	public BaseEntity()
	{
		Ordering = 10_000;

		CreatedDate = SeedWork.Utilities.DateTimeOffsetNow;
	}

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public TEntityKey? Id { get; set; }

	public DateTimeOffset? CreatedDate { get; set; }

	public int Ordering { get; set; }
}
