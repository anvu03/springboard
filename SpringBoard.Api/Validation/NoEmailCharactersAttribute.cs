using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SpringBoard.Api.Validation;

/// <summary>
/// Validation attribute that ensures a string does not contain characters that would make it look like an email address.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class NoEmailCharactersAttribute : ValidationAttribute
{
    private static readonly Regex EmailCharactersRegex = new(@"[@.]+", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="NoEmailCharactersAttribute"/> class.
    /// </summary>
    public NoEmailCharactersAttribute() 
        : base("The {0} field must not contain email-like characters (@ or .).")
    {
    }

    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A validation result.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
        {
            return ValidationResult.Success;
        }

        if (EmailCharactersRegex.IsMatch(stringValue))
        {
            var memberNames = new[] { validationContext.MemberName! };
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        return ValidationResult.Success;
    }
}
