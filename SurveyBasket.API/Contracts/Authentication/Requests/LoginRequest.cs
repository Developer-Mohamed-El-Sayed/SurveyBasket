namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record LoginRequest(
    string Email,
    string Password
);
