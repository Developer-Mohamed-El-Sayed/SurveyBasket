namespace SurveyBasket.API.Contracts.Users.Requests;

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName
);
