namespace SurveyBasket.API.Errors;

public static class PollErrors // use in all error record
{
    public static readonly Error PollNotFound =
        new("Poll.Invalid Poll", "Poll Not Found By Given Id", StatusCodes.Status404NotFound);
    public static readonly Error DublicatedTitle =
    new("Poll.Dublicated Title", "Poll Title is dublicated.", StatusCodes.Status409Conflict);
}
