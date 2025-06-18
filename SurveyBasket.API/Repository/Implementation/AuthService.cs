namespace SurveyBasket.API.Repository.Implementation;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, SignInManager<ApplicationUser> signInManager)
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly int _expirationRefreshTokenDays = 7;

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        var isValidPassword = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (isValidPassword is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var expirationRefreshTokenDays = DateTime.UtcNow.AddDays(_expirationRefreshTokenDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = expirationRefreshTokenDays
        });
        await _userManager.UpdateAsync(user);
        return Result.Success(new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, expirationRefreshTokenDays));
    }

    public async Task<Result<AuthResponse>> GenerateRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);

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
        var respones = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, newExpirationRefreshTokenDays);
        return Result.Success(respones);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if(userId is null) 
            return Result.Failure(UserErrors.InvalidUser);
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
            return Result.Failure(UserErrors.InvalidUser);
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);
        if (userRefreshToken is null) 
            return Result.Failure(UserErrors.InvalidUser);
        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(e => e.Email == request.Email,cancellationToken);
        if (emailIsExist)
            return Result.Failure<AuthResponse>(UserErrors.DublicatedEmail);
        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user,request.Password);
        // send confirmation email
        if(result.Succeeded)
            return Result.Success();
        var error   = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


}
