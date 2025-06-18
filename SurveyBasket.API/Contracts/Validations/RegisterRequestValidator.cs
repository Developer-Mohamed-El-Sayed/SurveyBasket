namespace SurveyBasket.API.Contracts.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(f => f.FirstName)
            .NotEmpty()
            .WithMessage("First Name required.");
        RuleFor(l => l.LastName)
            .NotEmpty()
            .WithMessage("Last Name Required.");
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage("email is required.")
            .EmailAddress()
            .WithMessage("invalid email format.");
        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
