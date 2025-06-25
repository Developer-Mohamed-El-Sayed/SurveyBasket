namespace SurveyBasket.API.Controllers;
[Route("[controller]/me")]
[ApiController]
[Authorize]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    [HttpGet]
    public async Task<IActionResult> Info()
    {
        var result = await _accountService.GetUserProfileAsync(User.GetUserId());
        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }
    [HttpPut("info")]
    public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserProfileRequest request)
    {
        var result = await _accountService.UpdateUserProfileAsync(User.GetUserId(), request);   
        return result.IsSuccess ? NoContent() : result.ToProblem() ; 
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _accountService.ChangePasswordAsync(User.GetUserId(), request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
} 
