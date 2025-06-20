﻿namespace SurveyBasket.API.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
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
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id,CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAsync(pollId,id,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] int pollId,CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAllAsync(pollId,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAvailableAsync(pollId,User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus([FromRoute]int pollId, [FromRoute]int id,CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId,id,cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]int pollId, [FromRoute]int id, [FromBody] QuestionRequest request,CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId ,id,request,cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
