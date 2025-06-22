namespace SurveyBasket.API.Contracts.Validations;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty();

        RuleFor(c => c.Code)
            .NotEmpty();
    }
}
