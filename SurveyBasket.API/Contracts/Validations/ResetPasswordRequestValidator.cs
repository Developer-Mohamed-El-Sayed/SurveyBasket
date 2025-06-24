namespace SurveyBasket.API.Contracts.Validations;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(e => e.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters.");

        RuleFor(p => p.NewPassword)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Matches(RegexPatterns.Password)
                .WithMessage("Password invalid Format.");

        RuleFor(c => c.Code)
               .NotEmpty();
    
    }
}
