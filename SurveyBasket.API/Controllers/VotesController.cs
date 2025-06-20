namespace SurveyBasket.API.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class VotesController(IVoteService voteService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    [HttpPost]
    public async Task<IActionResult> Create([FromRoute]int pollId, [FromBody] VoteRequest request,CancellationToken cancellationToken)
    {
        var result = await _voteService.CreateAsync(pollId, User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ?
            Created() : result.ToProblem();
    }
}
