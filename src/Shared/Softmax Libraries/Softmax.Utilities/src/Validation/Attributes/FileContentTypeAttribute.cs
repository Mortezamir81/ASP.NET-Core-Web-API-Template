using FileTypeChecker;
using FileTypeChecker.Abstracts;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Softmax.Utilities.Validation.Attributes;

public class FileContentTypeAttribute : ValidationAttribute
{
	private bool _isBitmapCheck = false;
	private bool _isImageCheck = false;
	private readonly string[]? _realExtensions;
	private const string _defualtMessage =
		"The content or extention of your upload file {0} extention is not allowed for ({1})";


	public FileContentTypeAttribute(bool isBitmap = false, bool isImage = false)
	{
		if (isBitmap == false && isImage == false)
			throw new ArgumentException
				(message: "You must pass at least one constructor input parameter");

		_isBitmapCheck = isBitmap;
		_isImageCheck = isImage;
	}

	public FileContentTypeAttribute(string[] realExtentions)
	{
		if (realExtentions == null || realExtentions.Length == 0)
			throw new ArgumentException
				(message: "Constracor parameter: realExtentions most not be null or empty");

		_realExtensions = realExtentions;
	}


	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var file = value as IFormFile;

		if (file != null)
			return CheckFile(file, validationContext);

		var files = value as IList<IFormFile>;

		if (files != null)
			return ValidationResult.Success;

		foreach (var currentFile in files)
		{
			var result = CheckFile(currentFile, validationContext);

			if (result != ValidationResult.Success)
				return result;
		}

		return ValidationResult.Success;
	}

	private ValidationResult? CheckFile(IFormFile file, ValidationContext validationContext)
	{
		var fileName = file.FileName;

		if (_realExtensions != null && _realExtensions.Length > 0)
		{
			var extension =
				Path.GetExtension(file.FileName);

			if (!_realExtensions.Contains(extension?.ToLower()))
			{
				return InvalidFileContent
					(fileName: fileName, propertyName: validationContext.DisplayName);
			}
		}

		using var fileStream = file.OpenReadStream();

		var isRecognizableType =
			FileTypeValidator.IsTypeRecognizable(fileStream);

		if (!isRecognizableType)
		{
			return InvalidFileContent
				(fileName: fileName, propertyName: validationContext.DisplayName);
		}

		IFileType fileType =
			FileTypeValidator.GetFileType(fileStream);

		if (_isBitmapCheck)
		{
			var isBitmap = fileStream.Is<Bitmap>();

			if (!isBitmap)
			{
				return InvalidFileContent
					(fileName: fileName, propertyName: validationContext.DisplayName);
			}
		}

		if (_isImageCheck)
		{
			var isImage = fileStream.IsImage();

			if (!isImage)
			{
				return InvalidFileContent
					(fileName: fileName, propertyName: validationContext.DisplayName);
			}
		}

		if (_realExtensions != null && _realExtensions.Length > 0)
		{
			var realExtention =
				fileType.Extension.Insert(0, ".").ToLower();

			if (!_realExtensions.Contains(realExtention))
			{
				return InvalidFileContent
					(fileName: fileName, propertyName: validationContext.DisplayName);
			}
		}

		return ValidationResult.Success;
	}

	private ValidationResult InvalidFileContent(string fileName, string propertyName)
	{
		return new ValidationResult
			(ErrorMessage ??
			string.Format(ErrorMessageString, fileName, propertyName) ??
			string.Format(_defualtMessage, fileName, propertyName));
	}
}
