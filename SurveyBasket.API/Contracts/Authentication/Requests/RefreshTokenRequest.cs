namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);
