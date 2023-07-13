using FluentValidation;

namespace AMS.Validators;

public static class HexValidator
{
    public static IRuleBuilderOptions<T, string> MustBeHexColor<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$").When(x => x != null).WithMessage("Invalid hex color format.");
    }
}