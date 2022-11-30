namespace ViewModels.General;

public class UserClaims
{
	public required string Id { get; set; }

	public required string RoleId { get; set; }

	public required string RoleName { get; set; }

	public required string SecurityStamp { get; set; }
}
