namespace SurveyBasket.API.Contracts.Validations;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(t => t.Token)
            .NotEmpty()
            .WithMessage("token required");

        RuleFor(r => r.RefreshToken)
         .NotEmpty()
         .WithMessage("Refresh Token required");
    }
}
