namespace SurveyBasket.API.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}
