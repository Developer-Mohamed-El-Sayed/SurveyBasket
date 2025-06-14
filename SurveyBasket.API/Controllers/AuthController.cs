namespace SurveyBasket.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request.Email, request.Password);
        return response is not null ? Ok(response) : BadRequest("Invalid Email Or Password");
    }
}
