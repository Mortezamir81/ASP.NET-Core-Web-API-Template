namespace Domain;

public class UserToken : BaseEntity<int>
{
	public User? User { get; set; }
	public int? UserId { get; set; }

	public DateTimeOffset? AccessTokenExpireDate { get; set; }
	public required string AccessTokenHash { get; set; }


	public DateTimeOffset? RefreshTokenExpireDate { get; set; }
	public required string RefreshTokenHash { get; set; }

	public string? CreatedByIp { get; set; }

	public bool IsRevoked { get; set; }
}
