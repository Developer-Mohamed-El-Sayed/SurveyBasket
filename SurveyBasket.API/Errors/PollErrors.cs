namespace SurveyBasket.API.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.Invalid Poll", "Poll Not Found By Given Id", StatusCodes.Status404NotFound);
}
