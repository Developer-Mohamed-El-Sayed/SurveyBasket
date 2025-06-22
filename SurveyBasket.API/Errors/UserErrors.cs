namespace SurveyBasket.API.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.Invalid Credentials", "Invalid email or password", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidUser =
        new("User.Invalid User", "Invalid user", StatusCodes.Status404NotFound);
    public static readonly Error DublicatedEmail =
        new("User.Dublicated Email User", "Dublicated Email", StatusCodes.Status409Conflict);
    public static readonly Error EmailNotConfirmed =
    new("User.EmailNotConfirmed", "Email Not Confirmed", StatusCodes.Status401Unauthorized);
    public static readonly Error LockOutUser =
    new("User.LockOutUser", "Lock Out User", StatusCodes.Status423Locked);
    public static readonly Error EmailConfirmed =
new("User.EmailConfirmed", "Email is already Confirmed", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidCode =
new("User.InvalidCode", " Invalid Code", StatusCodes.Status401Unauthorized);
}
