namespace Domain.UserManagment;

public class UserLogin : BaseEntity
{
	public UserLogin(Guid refreshToken) : base()
	{
		RefreshToken = refreshToken;
	}

	public User? User { get; set; }
	public int? UserId { get; set; }
	public DateTime? Expires { get; set; }
	public DateTime? Created { get; set; }
	public Guid RefreshToken { get; set; }
	public string? CreatedByIp { get; set; }
	public bool IsExpired
	{
		get
		{
			return DateTime.UtcNow > Expires;
		}
	}
}
