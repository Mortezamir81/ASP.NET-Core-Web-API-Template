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
	public async Task DeleteUserToken(int userTokenId)
	{
		await _databaseContext.UserAccessTokens!
			.Where(current => current.Id == userTokenId)
			.ExecuteDeleteAsync();
	}


	public async Task DeleteUserTokens(int userId)
	{
		if (_databaseContext.UserAccessTokens != null)
			await _databaseContext.UserAccessTokens
				.Where(current => current.UserId == userId)
				.ExecuteDeleteAsync();
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


	public async Task AddUserTokenAsync(UserToken userLogin)
	{
		await _databaseContext.UserAccessTokens!.AddAsync(userLogin);
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
