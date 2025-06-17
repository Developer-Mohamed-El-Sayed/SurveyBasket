namespace SurveyBasket.API.Extentions;

public static class UserExtention
{
    public static string GetUserId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
