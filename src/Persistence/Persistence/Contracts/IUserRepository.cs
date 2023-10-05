namespace Persistence.Repositories;

public interface IUserRepository
{
	Task<bool> CheckRoleExist(int roleId);


	Task<bool> CheckEmailExist(string? email);


	Task DeleteUserToken(int userTokenId);


	Task DeleteUserTokens(int userId);


	Task AddUserTokenAsync(UserToken userToken);


	Task<bool> CheckUserNameExist(string? username);


	Task<User?> GetUserById(int userId, bool isTracking);


	Task AddAsync
		(User entity, CancellationToken cancellationToken = default);


	Task<int> SaveChangesAsync();
}