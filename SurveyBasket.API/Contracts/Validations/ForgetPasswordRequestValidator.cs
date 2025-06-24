namespace SurveyBasket.API.Contracts.Validations;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
