namespace SurveyBasket.API.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] int pollId, [FromBody] QuestionRequest request,CancellationToken cancellationToken)
    {
        var result = await _questionService.CreateAsync(pollId,request,cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollId,result.Value.Id }, result.Value)
            : result.ToProblem();
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}
