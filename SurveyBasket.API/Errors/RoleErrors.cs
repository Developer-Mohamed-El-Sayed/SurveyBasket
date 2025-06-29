namespace SurveyBasket.API.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound =
        new("Role.InvalidRole", "the rolle not found by given id", StatusCodes.Status404NotFound);
    public static readonly Error  DublicatedRole =
        new("Role.DublicatedRole", "Dublicated Role", StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermissions =
    new("Role.InvalidPermissions", "Invalid Permissions Role", StatusCodes.Status404NotFound);

}
