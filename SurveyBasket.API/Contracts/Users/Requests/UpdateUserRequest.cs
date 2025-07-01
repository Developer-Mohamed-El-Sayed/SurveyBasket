namespace SurveyBasket.API.Contracts.Users.Requests;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles
);


