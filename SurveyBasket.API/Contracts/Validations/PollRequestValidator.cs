namespace SurveyBasket.API.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest> 
{
    public PollRequestValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage("requied")
            .Length(3,50)
            .WithMessage("min 3 , max 50");
    }
}
