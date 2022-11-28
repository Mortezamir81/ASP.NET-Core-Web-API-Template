namespace Domain.UserManagment;

public class Role
{
	public Role(string title)
	{
		Title = title;
	}

	public int Id { get; set; }
	public long? Price { get; set; }
	public string Title { get; set; }
}
