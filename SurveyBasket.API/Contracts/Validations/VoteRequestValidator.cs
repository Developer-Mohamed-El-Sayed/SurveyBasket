namespace SurveyBasket.API.Contracts.Validations;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(a => a.Answers)
            .NotEmpty()
            .WithMessage("Answers is required.");

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(x => x.Add(new VoteAnswerRequestValidator()));
    }
}
