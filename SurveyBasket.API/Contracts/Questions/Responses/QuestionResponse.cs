namespace SurveyBasket.API.Contracts.Questions.Responses;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
);
