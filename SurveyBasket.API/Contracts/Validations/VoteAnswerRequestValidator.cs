﻿namespace SurveyBasket.API.Contracts.Validations;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .GreaterThan(0);
        RuleFor(x => x.AnswerId)
           .NotEmpty()
           .GreaterThan(0);
    }
}
