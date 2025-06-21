namespace SurveyBasket.API.Contracts.Results.Responses;

public record VotePerAnswersResponse(
    string Answer,
    int Count
);
