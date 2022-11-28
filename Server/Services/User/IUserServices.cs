﻿namespace Services;

public interface IUserServices
{
	Task<Result> LogoutAsync(string token);


	Task<Result> UserSoftDeleteAsync(long? userId);


	Task<Result> RegisterAsync
		(RegisterRequestViewModel registerRequestViewModel);


	Task<Result>
		UpdateUserByAdminAsync(UpdateUserByAdminRequestViewModel viewModel);


	Task<Result<LoginResponseViewModel>>
		LoginAsync(LoginRequestViewModel loginRequestViewModel, string? ipAddress);


	Task<Result<LoginByOAuthResponseViewModel>>
		LoginByOAuthAsync(LoginByOAuthRequestViewModel requestViewModel, string? ipAddress);


	Task<Result>
		ChangeUserRoleAsync(ChangeUserRoleRequestViewModel changeUserRoleRequestViewModel);


	Task<Result<LoginResponseViewModel>> RefreshTokenAsync(string token, string? ipAddress);
}
