using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dtat.Utilities
{
	public class Security
	{
		public static string HashDataBySHA1(string data)
		{
			SHA1 sha1 = SHA1.Create();

			byte[] hashData =
				sha1.ComputeHash(Encoding.Default.GetBytes(data));

			StringBuilder returnValue = new StringBuilder();

			for (int i = 0; i < hashData.Length; i++)
			{
				returnValue.Append(hashData[i].ToString());
			}

			return returnValue.ToString();
		}


		public static string GetSha256(string text)
		{
			var inputBytes =
				System.Text.Encoding.UTF8.GetBytes(s: text);

			var sha =
				System.Security.Cryptography.SHA256.Create();

			var outputBytes =
				sha.ComputeHash(buffer: inputBytes);

			sha.Dispose();
			//sha = null;

			var result =
				System.Convert.ToBase64String(inArray: outputBytes);

			return result;
		}
	}
}
