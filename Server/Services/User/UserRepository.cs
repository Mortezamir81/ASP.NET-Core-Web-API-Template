namespace Services;

public partial class UserServices
{
	public async Task SaveChangesAsync()
	{
		await _databaseContext.SaveChangesAsync();
	}


	public void DeleteUser(User user)
	{
		_databaseContext.Users?.Remove(user);
	}


	public async Task AddUserAsync(User user)
	{
		await _databaseContext.Users!.AddAsync(user);
	}


	public void UpdateUserByAdmin(User user)
	{
		if (user == null)
			throw new ArgumentNullException(paramName: nameof(user));

		_databaseContext.Users?.Attach(user);
		_databaseContext.Entry(user).Property(x => x.FullName).IsModified = true;
		_databaseContext.Entry(user).Property(x => x.Username).IsModified = true;
		_databaseContext.Entry(user).Property(x => x.Email).IsModified = true;
		_databaseContext.Entry(user).Property(x => x.SecurityStamp).IsModified = true;
	}


	public async Task<bool> CheckRoleExist(int roleId)
	{
		var isRoleExist =
			await _databaseContext.Roles!
			.Select(current => current.Id)
			.Where(current => current == roleId)
			.AnyAsync();

		return isRoleExist;
	}


	public async Task<bool> CheckEmailExist(string? email)

	{
		var result =
			await _databaseContext.Users!
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
			await _databaseContext
				.Roles!
				.AsNoTracking()
				.Select(current => current.Id)
				.Where(current => current == roleId)
				.FirstOrDefaultAsync()
				;
	}


	public async Task AddUserLoginAsync(UserLogin userLogin)
	{
		await _databaseContext.UserLogins!.AddAsync(userLogin);
	}


	public async Task<bool> CheckUsernameExist(string? username)
	{
		var result =
			await _databaseContext.Users!
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
			await _databaseContext.Users!
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
				await _databaseContext
					.Users!
					.Where(current => current.Id == userId)
					.FirstOrDefaultAsync()
					;
		}
		else
		{
			user =
				await _databaseContext
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
				await _databaseContext.UserLogins!
					.Include(current => current.User)
					.Where(current => current.RefreshToken == refreshToken)
					.FirstOrDefaultAsync();
		}
		else
		{
			userLogin =
				await _databaseContext.UserLogins!
					.Where(current => current.RefreshToken == refreshToken)
					.FirstOrDefaultAsync();
		}

		return userLogin;
	}


	public async Task<LoginViewModel?> LoginAsync(string? username, string hashedPassword)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			throw new ArgumentNullException(paramName: username);
		}

		if (string.IsNullOrWhiteSpace(hashedPassword))
		{
			throw new ArgumentNullException(paramName: hashedPassword);
		}

		var result =
			await _databaseContext.Users!
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
}
