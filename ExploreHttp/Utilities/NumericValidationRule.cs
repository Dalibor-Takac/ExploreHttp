using System.Globalization;
using System.Windows.Controls;

namespace ExploreHttp.Utilities;
public class NumericValidationRule : ValidationRule
{
    public double MaximumValue { get; set; }
    public double MinimumValue { get; set; }
    public bool AllowFloatingPoint { get; set; }
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (AllowFloatingPoint)
        {
            if (double.TryParse(value as string, out var result))
            {
                if (result < MinimumValue)
                    return new ValidationResult(false, $"Must be greater than {MinimumValue}");
                if (result > MaximumValue)
                    return new ValidationResult(false, $"Must be less than {MaximumValue}");

                return ValidationResult.ValidResult;
            }
            else return new ValidationResult(false, "What is entered must be a number.");
        }
        else
        {
            if (int.TryParse(value as string, out var result))
            {
                if (result < MinimumValue)
                    return new ValidationResult(false, $"Must be greater than {MinimumValue}");
                if (result > MaximumValue)
                    return new ValidationResult(false, $"Must be less than {MaximumValue}");

                return ValidationResult.ValidResult;
            }
            else return new ValidationResult(false, "Supplied value must be whole number.");
        }
    }
}
