namespace Services;

public partial class UserServices
{
	#region Check Validation Methods

	public Result
		LogoutValidation(string token)
	{
		var result =
			new Result<LoginResponseViewModel>();

		if (string.IsNullOrWhiteSpace(token))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull, "RefreshToken");

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}


	public Result DeleteUserValidation
		(DeleteUserRequestViewModel deleteUserRequestViewModel)
	{
		var result =
			new Result();

		if (deleteUserRequestViewModel.Id == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(deleteUserRequestViewModel.Id));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		return result;
	}


	public Result<LoginResponseViewModel>
		RefreshTokenValidation(string token, string? ipAddress)
	{
		var result =
			new Result<LoginResponseViewModel>();

		if (string.IsNullOrWhiteSpace(token))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull, "RefreshToken");

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}


	public Result<LoginResponseViewModel>
		LoginValidation(LoginRequestViewModel loginRequestViewModel)
	{
		var result =
			new Result<LoginResponseViewModel>();

		if (loginRequestViewModel == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
				nameof(loginRequestViewModel));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		if (string.IsNullOrWhiteSpace(loginRequestViewModel.Username))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(loginRequestViewModel.Username));

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(loginRequestViewModel.Password))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(loginRequestViewModel.Password));

			result.AddErrorMessage(errorMessage);
		}

		if (result.IsFailed)
			return result;

		if (loginRequestViewModel.Username?.Length > Constants.MaxLength.CellPhoneNumber)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MaxLength,
					nameof(loginRequestViewModel.Username), Constants.MaxLength.CellPhoneNumber);

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}


	public Result<LoginByOAuthResponseViewModel>
		LoginByOAuthValidation(LoginByOAuthRequestViewModel requestViewModel)
	{
		var result =
			new Result<LoginByOAuthResponseViewModel>();

		if (requestViewModel == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
				nameof(requestViewModel));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		if (string.IsNullOrWhiteSpace(requestViewModel.Grant_Type))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(requestViewModel.Grant_Type));

			result.AddErrorMessage(errorMessage);
		}

		if (!requestViewModel.Grant_Type?.Equals("password", StringComparison.OrdinalIgnoreCase) == true)
		{
			string errorMessage = "OAuth flow is not password.";

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(requestViewModel.Username))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(requestViewModel.Username));

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(requestViewModel.Password))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(requestViewModel.Password));

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}


	public Result UpdateUserByAdminValidation
		(UpdateUserByAdminRequestViewModel viewModel)
	{
		var result =
			new Result();

		if (viewModel == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
				nameof(viewModel));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		if (viewModel.UserId <= 0)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserId);

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(viewModel.Username))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(viewModel.Username));

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(viewModel.Email))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(viewModel.Email));

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(viewModel.FullName))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(viewModel.FullName));

			result.AddErrorMessage(errorMessage);
		}

		if (viewModel.UserId <= 0)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserId);

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}


	public async Task<Result>
		RegisterValidation(RegisterRequestViewModel registerRequestViewModel)
	{
		var result =
			new Result();

		if (registerRequestViewModel == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
				nameof(registerRequestViewModel));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		if (string.IsNullOrWhiteSpace(registerRequestViewModel.Password))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(registerRequestViewModel.Password));

			result.AddErrorMessage(errorMessage);
		}

		if (string.IsNullOrWhiteSpace(registerRequestViewModel.Username))
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
					nameof(registerRequestViewModel.Username));

			result.AddErrorMessage(errorMessage);
		}

		if (result.IsFailed)
			return result;

		if (registerRequestViewModel.Username?.Length > Constants.MaxLength.CellPhoneNumber)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MaxLength,
					nameof(registerRequestViewModel.Username), Constants.MaxLength.EmailAddress);

			result.AddErrorMessage(errorMessage);
		}

		if (result.IsFailed)
			return result;

		var isPhoneNumberExist =
			await CheckUsernameExist(registerRequestViewModel.Username);

		if (isPhoneNumberExist)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.PhoneNumberExist);

			result.AddErrorMessage(errorMessage);
		}

		if (!string.IsNullOrWhiteSpace(registerRequestViewModel.Email))
		{
			var isEmailExist =
				await CheckEmailExist(registerRequestViewModel.Email);

			if (isEmailExist)
			{
				string errorMessage = string.Format
					(Resources.Messages.ErrorMessages.EmailExist);

				result.AddErrorMessage(errorMessage);
			}
		}

		return result;
	}


	public Result
		ChangeUserRoleValidation(ChangeUserRoleRequestViewModel changeUserRoleRequestViewModel)
	{
		var result =
			new Result<LoginResponseViewModel>();

		if (changeUserRoleRequestViewModel == null)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.MostNotBeNull,
				nameof(changeUserRoleRequestViewModel));

			result.AddErrorMessage(errorMessage);
			return result;
		}

		if (changeUserRoleRequestViewModel.UserId <= 0)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidUserId);

			result.AddErrorMessage(errorMessage);
		}

		if (changeUserRoleRequestViewModel.RoleId <= 0)
		{
			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidRoleId);

			result.AddErrorMessage(errorMessage);
		}

		return result;
	}
	#endregion /Check Validation Methods
}
