namespace SurveyBasket.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request.Email, request.Password,cancellationToken);
        return response is not null ? Ok(response) : BadRequest("Invalid Email Or Password");
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> GenerateRefreshToken ([FromBody] RefreshTokenRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.GenerateRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result is not null ? Ok(result) : BadRequest("invalid token || refresh Token.");
    }
    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result ? NoContent() : BadRequest("invalid token || refresh Token.");
    }
}

