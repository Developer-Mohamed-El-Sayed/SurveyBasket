namespace SurveyBasket.API.Repository.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string id);
}
