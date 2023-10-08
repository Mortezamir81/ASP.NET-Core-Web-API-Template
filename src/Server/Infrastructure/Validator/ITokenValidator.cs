namespace Infrastructure.Validator;

public interface ITokenValidator
{
	public Task ExecuteAsync(TokenValidatedContext context, JwtSettings jwtSettings);
}
