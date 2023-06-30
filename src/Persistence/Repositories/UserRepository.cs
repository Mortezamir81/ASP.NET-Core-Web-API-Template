namespace Persistence.Repositories;

public class UserRepository : IUserRepository, IRegisterAsScoped
{
	#region Properties
	private DatabaseContext _databaseContext { get; }

	private DbSet<User> _entities { get; }

	private IQueryable<User> _table => _entities;

	private IQueryable<User> _tableNoTracking => _entities.AsNoTracking();
	#endregion /Properties

	#region Constractor
	public UserRepository(DatabaseContext dbContext) : base()
	{
		_databaseContext = dbContext;

		_entities = _databaseContext.Set<User>();
	}
	#endregion /Constractor

	#region Methods
	public void DeleteUserLogin(UserLogin? userLogin)
	{
		Assert.NotNull(obj: userLogin, nameof(userLogin));

		_databaseContext.Remove(userLogin!);
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
			await _entities!
				.AsNoTracking()
				.Select(current => current.NormalizedEmail)
				.Where(current => current == email)
				.AnyAsync()
				;

		return result;
	}


	public async Task AddUserLoginAsync(UserLogin userLogin)
	{
		await _databaseContext.UserLogins!.AddAsync(userLogin);
	}


	public async Task<bool> CheckUserNameExist(string? username)
	{
		var result =
			await _databaseContext.Users!
				.AsNoTracking()
				.Select(current => current.NormalizedUserName)
				.Where(current => current == username)
				.AnyAsync()
				;

		return result;
	}


	public async Task<User?> GetUserById(int userId, bool isTracking)
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
		Assert.NotEmpty(obj: username, nameof(username));

		Assert.NotEmpty(obj: username, nameof(hashedPassword));

		//var result =
		//	await _databaseContext.Users!
		//		.AsNoTracking()
		//		.Where(current => current.UserName == username)
		//		.Where(current => current.PasswordHash == hashedPassword)
		//		.Select(current => new LoginViewModel(current.UserName, current.ro!.Value, current.UserRole!.Name)
		//		{
		//			Id = current.Id,
		//			SecurityStamp = current.SecurityStamp!,
		//			IsBanned = current.IsBanned,
		//		})
		//		.FirstOrDefaultAsync()
		//		;

		return null;
	}


	public async Task<int?> GetUserRoleAsync(int userId)
	{
		//var roleId =
		//	await _entities
		//	.Where(current => current.Id == userId)
		//	.Select(current => current.RoleId)
		//	.FirstOrDefaultAsync();

		//if (!roleId.HasValue || roleId == 0)
		//	return null;

		//return roleId;

		return null;
	}


	public async Task AddAsync
		(User entity, CancellationToken cancellationToken = default)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		await _entities.AddAsync(entity, cancellationToken);
	}

	public async Task<int> SaveChangesAsync()
	{
		return await _databaseContext.SaveChangesAsync();
	}
	#endregion /Methods

	#region Attach & Detach
	private void Detach(User entity)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		var entry = _databaseContext.Entry(entity);

		if (entry != null)
			entry.State = EntityState.Detached;
	}


	private void Attach(User entity)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		if (_databaseContext.Entry(entity).State == EntityState.Detached)
			_entities.Attach(entity);
	}
	#endregion /Attach&Detach
}
