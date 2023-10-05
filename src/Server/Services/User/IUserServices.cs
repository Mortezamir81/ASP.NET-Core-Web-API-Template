namespace Services;

public interface IUserServices
{
	Task<Result> LogoutAsync(int userTokenId, int userId);


	Task<Result> UserSoftDeleteAsync(int userId);


	Task<Result> ToggleBanUser(int userId);


	Task<Result> RegisterAsync
		(RegisterRequestViewModel registerRequestViewModel);


	Task<Result> UpdateUserByAdminAsync
			(UpdateUserByAdminRequestViewModel viewModel, int adminId);


	Task<Result<LoginResponseViewModel>>
		LoginAsync(LoginRequestViewModel loginRequestViewModel, string? ipAddress);


	Task<Result> ChangeUserRoleAsync
		(ChangeUserRoleRequestViewModel changeUserRoleRequestViewModel, int adminId);
}
