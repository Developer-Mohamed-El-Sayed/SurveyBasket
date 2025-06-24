namespace SurveyBasket.API.Contracts.Users.Requests;

public record ChangePasswordRequest(
    string Password,
    string NewPassword
);
