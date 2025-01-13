using System.Security.Cryptography;

namespace Infrastructure.Utilities;

public class SecurityHelper
{
	public static string ToSha256(string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return string.Empty;

		var inputBytes = Encoding.UTF8.GetBytes(input);

		var inputHash = SHA256.HashData(inputBytes);

		return Convert.ToHexString(inputHash);
	}
}
