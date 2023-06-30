using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Softmax.Utilities.Validation.Attributes
{
	public class AllowedExtensionsAttribute : ValidationAttribute
	{
		private readonly string[] _extensions;
		private const string _defualtMessage = "This extension is not allowed!";

		public AllowedExtensionsAttribute(string[] extensions)
		{
			_extensions = extensions;
		}


		protected override ValidationResult
			IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;

			if (file != null)

			{
				var extension =
					Path.GetExtension(file.FileName);

				if (!_extensions.Contains(extension?.ToLower()))
				{
					return new ValidationResult
						(string.Format(ErrorMessageString, extension) ?? ErrorMessage ?? _defualtMessage);
				}
			}

			return ValidationResult.Success;
		}
	}
}
