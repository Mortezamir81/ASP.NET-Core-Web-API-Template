namespace Domain;

public class Role : IdentityRole<int>
{
	public Role(string name)
	{
		Name = name;
	}
}
