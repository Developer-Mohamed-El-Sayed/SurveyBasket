namespace SurveyBasket.API.Contracts.Validations;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(n => n.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .NotEqual(x => x.Password);

    }
}
