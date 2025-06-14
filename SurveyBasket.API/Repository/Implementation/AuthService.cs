namespace SurveyBasket.API.Repository.Implementation;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, SignInManager<ApplicationUser> signInManager)
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return null;
        var isValidPassword = await _signInManager.PasswordSignInAsync(user, password, false, true);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        return new AuthResponse(user.Id,user.FirstName,user.LastName,user.Email,token,expiresIn);
    }
}
