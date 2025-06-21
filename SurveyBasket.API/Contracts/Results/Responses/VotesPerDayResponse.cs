namespace SurveyBasket.API.Contracts.Results.Responses;

public record VotesPerDayResponse(
    DateOnly Date,
    int NumberOfVotes
);
