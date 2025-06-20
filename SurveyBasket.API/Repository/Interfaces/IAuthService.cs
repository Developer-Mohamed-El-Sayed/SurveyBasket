﻿namespace SurveyBasket.API.Repository.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password,CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GenerateRefreshTokenAsync(string token,string refreshToken,CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
