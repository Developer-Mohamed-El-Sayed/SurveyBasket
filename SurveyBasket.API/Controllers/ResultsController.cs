namespace SurveyBasket.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;
    [HttpGet("row-data")]
    public async Task<IActionResult> Get([FromRoute] int pollId,CancellationToken cancellationToken)
    {
        var result = await _resultService.GetPollVotesAsync(pollId,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
