namespace SurveyBasket.API.Contracts.Results.Responses;

public record PollVoteResponse(
    string Title,
    string Summary,
    IEnumerable<VoteResponse> Votes
);
