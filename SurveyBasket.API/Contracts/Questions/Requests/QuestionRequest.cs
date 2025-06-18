namespace SurveyBasket.API.Contracts.Questions.Requests;

public record QuestionRequest(
    string Content,
    List<string> Answers
);
