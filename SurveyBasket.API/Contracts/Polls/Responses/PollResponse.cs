namespace SurveyBasket.API.Contracts.Polls.Response;

public record PollResponse(
    int Id,
    string Title,
    string Description
);
