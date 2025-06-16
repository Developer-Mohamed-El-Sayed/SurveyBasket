namespace SurveyBasket.API.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest> 
{
    public PollRequestValidator()
    {
        RuleFor(t => t.Title)
          .NotEmpty()
          .WithMessage("Title is required.")
          .Length(3, 100)
          .WithMessage("Title must be between 3 and 100 characters.");

        RuleFor(s => s.Summary)
            .NotEmpty()
            .WithMessage("Summary is required.")
            .Length(3, 1500)
            .WithMessage("Summary must be between 3 and 1500 characters.");

        RuleFor(s => s.StartsAt)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date must be today or a future date.");

        RuleFor(e => e.EndsAt)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(m => m)
            .Must(HasValidDate)
            .WithName(nameof(PollRequest.EndsAt))
            .WithMessage("End date must be after or equal to the start date.");




    }
    private bool HasValidDate(PollRequest request) => request.EndsAt >= request.StartsAt;
}
