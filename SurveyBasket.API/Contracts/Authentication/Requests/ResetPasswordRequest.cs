namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record ResetPasswordRequest(
    string Email,
    string NewPassword,
    string Code
);
