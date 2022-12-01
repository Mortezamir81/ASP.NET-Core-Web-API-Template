using Shared.Enums;

namespace Persistence.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
	void UpdateUserByAdmin(User user);


	Task<bool> CheckRoleExist(int roleId);


	Task<bool> CheckEmailExist(string? email);


	void DeleteUserLogin(UserLogin userLogin);


	Task<int> GetRoleIdInDatabaseAsync(int roleId);


	Task AddUserLoginAsync(UserLogin userLogin);


	Task<bool> CheckUsernameExist(string? username);


	Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);


	Task<User?> GetUserById(long userId, bool isTracking);


	Task<UserLogin?> GetUserLoginsAsync(Guid refreshToken, bool includeUser);


	Task<LoginViewModel?> LoginAsync(string? username, string hashedPassword);


	Task<int?> GetUserRoleAsync(int userId);
}