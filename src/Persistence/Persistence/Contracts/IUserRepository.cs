namespace Persistence.Repositories;

public interface IUserRepository
{
	Task<bool> CheckRoleExist(int roleId);


	Task<bool> CheckEmailExist(string? email);


	void DeleteUserLogin(UserLogin userLogin);


	Task AddUserLoginAsync(UserLogin userLogin);


	Task<bool> CheckUserNameExist(string? username);


	Task<User?> GetUserById(int userId, bool isTracking);


	Task<UserLogin?> GetUserLoginsAsync(Guid refreshToken, bool includeUser);


	Task<LoginViewModel?> LoginAsync(string? username, string hashedPassword);


	Task<int?> GetUserRoleAsync(int userId);


	Task AddAsync
		(User entity, CancellationToken cancellationToken = default);


	Task<int> SaveChangesAsync();
}