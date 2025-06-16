namespace SurveyBasket.API.Repository.Implementation;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, SignInManager<ApplicationUser> signInManager)
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly int _expirationRefreshTokenDays = 7;

    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return null;
        var isValidPassword = await _signInManager.PasswordSignInAsync(user, password, false, true);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var expirationRefreshTokenDays = DateTime.UtcNow.AddDays(_expirationRefreshTokenDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = expirationRefreshTokenDays
        });
        await _userManager.UpdateAsync(user);
        return new AuthResponse(user.Id,user.FirstName,user.LastName,user.Email,token,expiresIn,refreshToken,expirationRefreshTokenDays);
    }

    public async Task<AuthResponse> GenerateRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return null;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);
        if (userRefreshToken is null)
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var newExpirationRefreshTokenDays = DateTime.UtcNow.AddDays(_expirationRefreshTokenDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = newExpirationRefreshTokenDays
        });
        await _userManager.UpdateAsync(user);
        return new AuthResponse(user.Id,user.FirstName,user.LastName,user.Email,newToken,expiresIn,newRefreshToken,newExpirationRefreshTokenDays);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if(userId is null) return false;
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
            return false;
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);
        if (userRefreshToken is null) return false;
        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return true;
    }
    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
