namespace SurveyBasket.API.Repository.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string email, string password,CancellationToken cancellationToken = default);
}
