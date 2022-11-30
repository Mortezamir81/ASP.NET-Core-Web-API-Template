namespace Persistence.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
	#region Constractor
	public UserRepository(DatabaseContext dbContext) : base(dbContext)
	{
	}
	#endregion /Constractor

	#region Methods
	public void UpdateUserByAdmin(User user)
	{
		Assert.NotNull(obj: user, nameof(user));

		Attach(user);

		DatabaseContext.Entry(user).Property(x => x.FullName).IsModified = true;
		DatabaseContext.Entry(user).Property(x => x.Username).IsModified = true;
		DatabaseContext.Entry(user).Property(x => x.Email).IsModified = true;
		DatabaseContext.Entry(user).Property(x => x.SecurityStamp).IsModified = true;
	}


	public void DeleteUserLogin(UserLogin? userLogin)
	{
		Assert.NotNull(obj: userLogin, nameof(userLogin));

		DatabaseContext.Remove(userLogin!);
	}


	public async Task<bool> CheckRoleExist(int roleId)
	{
		var isRoleExist =
			await DatabaseContext.Roles!
			.Select(current => current.Id)
			.Where(current => current == roleId)
			.AnyAsync();

		return isRoleExist;
	}


	public async Task<bool> CheckEmailExist(string? email)

	{
		var result =
			await Entities!
				.AsNoTracking()
				.Select(current => current.Email)
				.Where(current => current == email)
				.AnyAsync()
				;

		return result;
	}


	public async Task<int> GetRoleAsync(int roleId)
	{
		return
			await DatabaseContext.Roles!
				.AsNoTracking()
				.Select(current => current.Id)
				.Where(current => current == roleId)
				.FirstOrDefaultAsync()
				;
	}


	public async Task AddUserLoginAsync(UserLogin userLogin)
	{
		await DatabaseContext.UserLogins!.AddAsync(userLogin);
	}


	public async Task<bool> CheckUsernameExist(string? username)
	{
		var result =
			await DatabaseContext.Users!
				.AsNoTracking()
				.Select(current => current.Username)
				.Where(current => current == username)
				.AnyAsync()
				;

		return result;
	}


	public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
	{
		return
			await DatabaseContext.Users!
				.AsNoTracking()
				.Where(current => current.Username.ToLower() == phoneNumber.ToLower())
				.FirstOrDefaultAsync()
				;
	}


	public async Task<User?> GetUserById(long userId, bool isTracking)
	{
		User? user = null;

		if (isTracking)
		{
			user =
				await DatabaseContext
					.Users!
					.Where(current => current.Id == userId)
					.FirstOrDefaultAsync()
					;
		}
		else
		{
			user =
				await DatabaseContext
					.Users!
					.AsNoTracking()
					.Where(current => current.Id == userId)
					.FirstOrDefaultAsync()
					;
		}

		return user;
	}


	public async Task<UserLogin?> GetUserLoginsAsync(Guid refreshToken, bool includeUser)
	{
		UserLogin? userLogin = null;

		if (includeUser)
		{
			userLogin =
				await DatabaseContext.UserLogins!
					.Include(current => current.User)
					.Where(current => current.RefreshToken == refreshToken)
					.FirstOrDefaultAsync();
		}
		else
		{
			userLogin =
				await DatabaseContext.UserLogins!
					.Where(current => current.RefreshToken == refreshToken)
					.FirstOrDefaultAsync();
		}

		return userLogin;
	}


	public async Task<LoginViewModel?> LoginAsync(string? username, string hashedPassword)
	{
		Assert.NotEmpty(obj: username, nameof(username));

		Assert.NotEmpty(obj: username, nameof(hashedPassword));

		var result =
			await DatabaseContext.Users!
				.AsNoTracking()
				.Where(current => current.Username == username)
				.Where(current => current.HashedPassword == hashedPassword)
				.Select(current => new LoginViewModel(current.Username, current.RoleId!.Value, current.UserRole!.Title)
				{
					Id = current.Id,
					SecurityStamp = current.SecurityStamp!.Value,
					IsBanned = current.IsBanned,
				})
				.FirstOrDefaultAsync()
				;

		return result;
	}
	#endregion /Methods
}
