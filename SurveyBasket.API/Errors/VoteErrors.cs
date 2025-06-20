namespace SurveyBasket.API.Errors;

public static class VoteErrors
{
    public static readonly Error DublicatedVote =
        new("Vote.DublicatedVote", "Dublicated Vote With the same id", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions =
       new("Vote.InvalidQuestions", "Invalid Question", StatusCodes.Status404NotFound);
}
