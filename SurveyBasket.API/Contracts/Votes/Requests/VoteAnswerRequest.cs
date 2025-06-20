namespace SurveyBasket.API.Contracts.Votes.Requests;

public record VoteAnswerRequest(
    int QuestionId,
    int AnswerId
);
