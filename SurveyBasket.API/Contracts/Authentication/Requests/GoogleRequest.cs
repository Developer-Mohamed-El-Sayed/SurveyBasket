namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record GoogleRequest(
    string IdToken,
    string Provider = "GOOGLE"
);
