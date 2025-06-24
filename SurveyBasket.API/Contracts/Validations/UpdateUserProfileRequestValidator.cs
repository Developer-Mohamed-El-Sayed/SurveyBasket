namespace SurveyBasket.API.Contracts.Validations;

public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(f => f.FirstName)
            .NotEmpty();

        RuleFor(l =>  l.LastName)
            .NotEmpty();
    }
}
