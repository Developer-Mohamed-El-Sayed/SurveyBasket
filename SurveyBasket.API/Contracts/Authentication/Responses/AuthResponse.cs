namespace SurveyBasket.API.Contracts.Authentication.Responses;

public record AuthResponse(
    string Id,
    string FirstName,
    string LastName,
    string? Email,
    string Token,
    int ExpiresIn
);
