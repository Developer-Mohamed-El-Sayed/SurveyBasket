namespace SurveyBasket.API.Repository.Interfaces;

public interface IAccountService
{
    Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
    Task<Result> UpdateUserProfileAsync(string userId,UpdateUserProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request);
}
