namespace SurveyBasket.API.Contracts.Validations;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.");

        RuleFor(f => f.FirstName)
            .NotEmpty()
            .WithMessage("First Name required.");

        RuleFor(l => l.LastName)
            .NotEmpty()
            .WithMessage("Last Name Required.");

        RuleFor(r => r.Roles)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Distinct().Count() == x.Count)
            .When(r => r.Roles != null);
    }
}
