namespace SurveyBasket.API.Errors;

public static class QuestionErrors
{
    public static readonly Error DublicatedQuestions =
        new("Question.DublicatedQuestion", "Dublicated Question with the same Poll.", StatusCodes.Status409Conflict);
}
