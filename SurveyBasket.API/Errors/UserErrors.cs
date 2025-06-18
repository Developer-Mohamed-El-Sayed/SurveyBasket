namespace SurveyBasket.API.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.Invalid Credentials", "Invalid email or password", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidUser =
        new("User.Invalid User", "Invalid user", StatusCodes.Status404NotFound);
    public static readonly Error DublicatedEmail =
        new("User.Dublicated Email User", "Dublicated Email", StatusCodes.Status409Conflict);
}
