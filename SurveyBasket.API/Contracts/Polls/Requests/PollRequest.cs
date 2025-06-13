namespace SurveyBasket.API.Contracts.Polls.Request;

public record PollRequest(
    string Title,
    string Description
);