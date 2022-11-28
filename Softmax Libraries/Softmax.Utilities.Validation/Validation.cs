using System.Net.Mail;

namespace Dtat.Utilities
{
	public class Validation
	{
		public static bool CheckEmailValid(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			if (email.Trim().EndsWith("."))
			{
				return false;
			}
			try
			{
				var addr = new MailAddress(email.Trim());
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
