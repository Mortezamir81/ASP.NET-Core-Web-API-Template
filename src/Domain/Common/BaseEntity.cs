namespace Domain.Common;

public class BaseEntity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int? Id { get; set; }
	public DateTime? CreatedDate { get; set; }
}
