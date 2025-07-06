namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(DefaultRateLimit.TokenLimit)]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    [HttpGet]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
       return result.IsSuccess ? Ok(result.Value): result.ToProblem();  
    }
    [Authorize(Roles = DefaultRoles.Member)]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetCurrentAsync(cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost]
    [HasPermission(Permissions.CreatePolls)]
    public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HasPermission(Permissions.DeletePolls)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]int id,CancellationToken cancellationToken = default)
    {
       var result = await _pollService.DeleteAsync(id,cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("{id}/toggle-status")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> ToggleStatus([FromRoute] int id,CancellationToken cancellationToken)
    {
        var result = await _pollService.ToggleStatusAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
