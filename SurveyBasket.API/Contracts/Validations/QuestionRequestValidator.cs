namespace SurveyBasket.API.Contracts.Validations;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty()
            .WithMessage("content is required.")
            .Length(3, 1000)
            .WithMessage("the min length 3 , max length 1000.");



        RuleFor(a => a.Answers)
            .NotEmpty()
            .WithMessage("answers is required.");

        RuleFor(a => a.Answers)
            .Must(a => a.Count > 1)
            .WithMessage("question should be at least 2 answers")
            .When(x => x.Answers != null);

        RuleFor(a => a.Answers)
            .Must(a => a.Distinct().Count() == a.Count)
            .WithMessage("you cannot add dublicated answers for the same question")
            .When(x => x.Answers != null);
    }
}
