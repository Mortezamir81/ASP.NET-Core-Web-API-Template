using Dtat.Result;

namespace Services;

public interface IUserServices
{
	Task<Result> LogoutAsync(string token);


	Task<Result> UserSoftDeleteAsync(int userId);


	Task<Result> ToggleBanUser(int userId);


	Task<Result> RegisterAsync
		(RegisterRequestViewModel registerRequestViewModel);


	Task<Result>UpdateUserByAdminAsync
			(UpdateUserByAdminRequestViewModel viewModel, int adminId);


	Task<Result<LoginResponseViewModel>>
		LoginAsync(LoginRequestViewModel loginRequestViewModel, string? ipAddress);


	Task<Result<LoginByOAuthResponseViewModel>>
		LoginByOAuthAsync(LoginByOAuthRequestViewModel requestViewModel, string? ipAddress);


	Task<Result>ChangeUserRoleAsync
		(ChangeUserRoleRequestViewModel changeUserRoleRequestViewModel, int adminId);


	Task<Result<LoginResponseViewModel>> RefreshTokenAsync(string token, string? ipAddress);
}
