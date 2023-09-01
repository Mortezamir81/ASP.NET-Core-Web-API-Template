using System.ComponentModel.DataAnnotations;

namespace Softmax.Utilities.Validation.Attributes;

public class MinValueAttribute : ValidationAttribute
{
	private readonly int _minValue;
	private const string _defualtMessage = "The property {0} value must grather than {1}";

	public MinValueAttribute(int minValue)
	{
		_minValue = minValue;
	}


	protected override ValidationResult?
		IsValid(object? value, ValidationContext validationContext)
	{
		if (value == null)
			return ValidationResult.Success;

		long? propertyValue = null;

		if (value.GetType() == typeof(int) ||
			value.GetType() == typeof(long) ||
			value.GetType() == typeof(int?) ||
			value.GetType() == typeof(long?))
		{
			propertyValue = Convert.ToInt64(value);
		}

		if (!propertyValue.HasValue)
			return ValidationResult.Success;

		if (propertyValue < _minValue)
		{
			return new ValidationResult
				(ErrorMessage ??
				string.Format(ErrorMessageString, validationContext.DisplayName, _minValue - 1) ??
				string.Format(_defualtMessage, validationContext.DisplayName, _minValue - 1));
		}

		return ValidationResult.Success;
	}
}
