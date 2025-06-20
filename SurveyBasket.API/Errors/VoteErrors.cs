namespace SurveyBasket.API.Errors;

public static class VoteErrors
{
    public static readonly Error DublicatedVote =
        new("Vote.DublicatedVote", "Dublicated Vote With the same id", StatusCodes.Status409Conflict);
}
