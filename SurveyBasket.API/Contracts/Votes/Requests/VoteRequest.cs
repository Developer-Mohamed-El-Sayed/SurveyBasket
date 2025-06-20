namespace SurveyBasket.API.Contracts.Votes.Requests;

public record VoteRequest(
    IEnumerable<VoteAnswerRequest> Answers
);
