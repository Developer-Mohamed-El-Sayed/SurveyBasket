namespace SurveyBasket.API.Errors;

public static class VoteErrors
{
    public static readonly Error DublicatedVote =
        new("Vote.DublicatedVote", "Dublicated Vote With the same id", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions =
       new("Vote.InvalidQuestions", "Invalid Question", StatusCodes.Status404NotFound);
    public static readonly Error VoteNotFound =
        new("Vote.NotFound", "Invalid Vote with poll by given id", StatusCodes.Status404NotFound);
}
