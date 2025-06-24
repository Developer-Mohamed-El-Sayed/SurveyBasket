namespace SurveyBasket.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> GenerateRefreshToken ([FromBody] RefreshTokenRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.GenerateRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
        return result.IsSuccess ? Accepted() : result.ToProblem();
    }
    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);
        return result.IsSuccess ? Accepted() : result.ToProblem();
    }
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleRequest request)
    {
        var result = await _authService.LoginGoogleAsync(request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var result = await _authService.SendForgetPasswordCodeAsync(request);
        return result.IsSuccess ? Accepted() : result.ToProblem();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}

