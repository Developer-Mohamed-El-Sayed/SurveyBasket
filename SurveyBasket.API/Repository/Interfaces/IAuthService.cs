namespace SurveyBasket.API.Repository.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string email, string password,CancellationToken cancellationToken = default);
    Task<AuthResponse> GenerateRefreshTokenAsync(string token,string refreshToken,CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}
