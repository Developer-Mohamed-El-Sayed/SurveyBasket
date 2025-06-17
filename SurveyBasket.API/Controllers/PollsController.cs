namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
       return result.IsSuccess ? Ok(result.Value): result.ToProblem();  
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { Id = result.Value.Id }, request);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]int id,CancellationToken cancellationToken = default)
    {
       var result = await _pollService.DeleteAsync(id,cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
