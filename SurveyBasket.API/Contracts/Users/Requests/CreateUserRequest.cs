namespace SurveyBasket.API.Contracts.Users.Requests;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IList<string> Roles
);
