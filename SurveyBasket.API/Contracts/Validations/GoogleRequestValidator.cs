namespace SurveyBasket.API.Contracts.Validations;

public class GoogleRequestValidator : AbstractValidator<GoogleRequest>
{
    public GoogleRequestValidator()
    {
        RuleFor(i => i.IdToken)
            .NotEmpty();

        RuleFor(p => p.Provider)
            .NotEmpty();
    }
}
