namespace SurveyBasket.API.Contracts.Results.Responses;

public record VotePerQuestionResponse(
    string Question,
    IEnumerable<VotePerAnswersResponse> SelectedAnswers
);
