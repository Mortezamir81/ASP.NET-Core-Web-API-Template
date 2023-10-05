namespace Domain;

public class UserToken : BaseEntity<int>
{
	public User? User { get; set; }
	public int? UserId { get; set; }
	public DateTimeOffset? ExpireDate { get; set; }
	public required string AccessToken { get; set; }
	public string? CreatedByIp { get; set; }
}
