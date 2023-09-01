namespace Domain;

public class ApplicationFile : BaseEntity<int>
{
	public ApplicationFile() : base()
	{

	}

	#region Auto Generated Properties
	public long Size { get; set; }

	public string? ContentType { get; set; }

	public string? Description { get; set; }

	public required string Name { get; set; }

	public required string Path { get; set; }

	public required string Extension { get; set; }
	#endregion /Auto Generated Properties
}