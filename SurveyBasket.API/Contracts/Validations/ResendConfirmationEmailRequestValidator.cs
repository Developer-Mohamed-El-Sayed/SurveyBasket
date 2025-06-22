namespace SurveyBasket.API.Contracts.Validations;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>

{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage("email is required.")
            .EmailAddress()
            .WithMessage("invalid email format.");
    }
}
