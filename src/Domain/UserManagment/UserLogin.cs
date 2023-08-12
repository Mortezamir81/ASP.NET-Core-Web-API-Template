namespace Domain.UserManagment;

public class UserLogin : BaseEntity<int>
{
	public UserLogin(Guid refreshToken) : base()
	{
		RefreshToken = refreshToken;
	}

	public User? User { get; set; }
	public int? UserId { get; set; }
	public DateTimeOffset? Expires { get; set; }
	public DateTimeOffset? Created { get; set; }
	public Guid RefreshToken { get; set; }
	public string? CreatedByIp { get; set; }
	public bool IsExpired
	{
		get
		{
			return Domain.SeedWork.Utilities.DateTimeOffsetNow > Expires;
		}
	}
}
