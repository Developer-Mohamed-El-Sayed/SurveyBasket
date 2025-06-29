namespace SurveyBasket.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _roleService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var result = await _roleService.GetAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost]
    [HasPermission(Permissions.CreateRoles)]
    public async Task<IActionResult> Create([FromBody] RoleRequest request,CancellationToken cancellationToken)
    {
        var result =  await _roleService.CreateAsync(request,cancellationToken);
        return result.IsSuccess ?
            CreatedAtAction(nameof(Get), new { result.Value.Id }, result)
            : result.ToProblem();
    }

}
