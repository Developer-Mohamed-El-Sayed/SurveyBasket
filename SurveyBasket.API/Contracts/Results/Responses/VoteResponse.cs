namespace SurveyBasket.API.Contracts.Results.Responses;

public record VoteResponse(
    string VoterName,
    DateTime VottingDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);
