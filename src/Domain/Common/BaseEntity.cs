namespace Domain.Common;

public class BaseEntity<TEntityKey>
{
	public BaseEntity()
	{
		Ordering = 10_000;
	}

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public TEntityKey? Id { get; set; }

	public DateTime? CreatedDate { get; set; }

	public int Ordering { get; set; }
}
