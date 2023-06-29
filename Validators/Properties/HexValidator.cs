using FluentValidation;

namespace AMS.Validators;

public static class HexValidator
{
    public static IRuleBuilderOptions<T, string> MustBeHexColor<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Hex color must not be empty.")
            .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$").WithMessage("Invalid hex color format.");
    }
}