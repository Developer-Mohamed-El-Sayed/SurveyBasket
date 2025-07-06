namespace SurveyBasket.API.Repository.Implementations;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IOptions<GoogleSettings> options,
    SurveyBasketDbContext context
    )
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly SurveyBasketDbContext _context = context;
    private readonly GoogleSettings _options = options.Value;
    private readonly int _expirationRefreshTokenDays = 7;

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (result.Succeeded)
        {
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userPermissions, userRoles);
            var refreshToken = GenerateRefreshToken();
            var expirationRefreshTokenDays = DateTime.UtcNow.AddDays(_expirationRefreshTokenDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = expirationRefreshTokenDays
            });
            await _userManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, expirationRefreshTokenDays);
            return Result.Success(response);
        }
        return Result.Failure<AuthResponse>(
            result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
            ? UserErrors.LockOutUser
            : UserErrors.InvalidCredentials
        );
    }

    public async Task<Result<AuthResponse>> GenerateRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockOutUser);


        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userPermissions, userRoles);
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

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidUser);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);
        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        }
        catch (FormatException)
        {

            return Result.Failure(UserErrors.InvalidCode);
        }
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success(result);
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidUser);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
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
        var emailIsExist = await _userManager.Users.AnyAsync(e => e.Email == request.Email, cancellationToken);
        if (emailIsExist)
            return Result.Failure<AuthResponse>(UserErrors.DublicatedEmail);
        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        // send confirmation email
        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


            _logger.LogInformation("confirmation Code {code} ", code);
            await SendConfirmationToEmail(user, code);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success(); // for security
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        _logger.LogInformation("Resend Confirmation Code: {code} ", code);

        await SendConfirmationToEmail(user, code);
        return Result.Success();
    }
    public async Task<Result<AuthResponse>> LoginGoogleAsync(GoogleRequest request, CancellationToken cancellationToken = default)
    {
        if (await VerifyGoogleToken(request.IdToken) is not { } payload)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);
        var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);
        if (await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUser);
        user = await _userManager.FindByEmailAsync(payload.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        user = new ApplicationUser
        {
            Email = payload.Email,
            UserName = payload.Email,
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user);
        await _userManager.AddLoginAsync(user, info);
        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userPermissions, userRoles);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiratons = DateTime.UtcNow.AddDays(_expirationRefreshTokenDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiratons
        });
        await _userManager.UpdateAsync(user);
        var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, refreshTokenExpiratons);
        return Result.Success(response);
    }
    public async Task<Result> SendForgetPasswordCodeAsync(ForgetPasswordRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success(); // misleading for hacking 
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("forget password code: {code}", code);
        await SendForgetPasswordCodeToEmail(user, code);
        return Result.Success();

    }
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(UserErrors.InvalidCredentials);
        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode); // misleading email not confirmed
        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);

        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }
    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private async Task SendConfirmationToEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            template: "EmailConfirmation",
            new Dictionary<string, string>
            {
                    {"{{Name}}",$"{user.FirstName} {user.LastName}" },
                    {"{{action_url}}", $"{origin}/auth/confirmationEmail?userId={user.Id}&code={code}"}
            }
        );
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Email Confirmation Done ✅", emailBody));
        await Task.CompletedTask;
    }
    private async Task SendForgetPasswordCodeToEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            template: "ForgetPassword",
            new Dictionary<string, string>
            {
                    {"{{Name}}",$"{user.FirstName} {user.LastName}" },
                    {"{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}"}
            }
        );
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Email Confirmation Done ✅", emailBody));
        await Task.CompletedTask;
    }
    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_options.ClientId]
        };
        try
        {
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch
        {

            return null!;
        }
    }
    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await _context.Roles
            .Join(_context.RoleClaims,
                role => role.Id,
                claim => claim.RoleId,
                (role, claim) => new { role, claim }
            )
            .Where(x => userRoles.Contains(x.role.Name!))
            .Select(x => x.claim.ClaimValue!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }
}
