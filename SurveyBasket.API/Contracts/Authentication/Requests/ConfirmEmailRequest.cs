namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record ConfirmEmailRequest(
    string Code,
    string UserId // Client
);
