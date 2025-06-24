namespace SurveyBasket.API.Contracts.Users.Responses;

public record UserProfileResponse(
    string FirstName,
    string LastName,
    string Email
);
