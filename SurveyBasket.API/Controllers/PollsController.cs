namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var respone = await _pollService.GetAllAsync(cancellationToken);
        return Ok(respone);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var response = await _pollService.GetAsync(id, cancellationToken);
        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _pollService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { Id = response.Id }, request);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        await _pollService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]int id,CancellationToken cancellationToken = default)
    {
        await _pollService.DeleteAsync(id,cancellationToken);
        return NoContent();
    }
}
