namespace SurveyBasket.API.Repository.Implementations;

public class RoleService(RoleManager<ApplicationRole> roleManager,SurveyBasketDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly SurveyBasketDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default) => 
        await _roleManager.Roles
        .Where(x => !x.IsDefault)
        .AsNoTracking()
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);
    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);
        var permissions = await _roleManager
            .GetClaimsAsync(role);
        var response = new RoleDetailResponse(id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));
        return Result.Success(response);
    }
    public async Task<Result<RoleDetailResponse>> CreateAsync(RoleRequest request,CancellationToken cancellationToken = default)
    {
        var roleIsExist = await _roleManager.RoleExistsAsync(request.Name);

        if(roleIsExist)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DublicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);
        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
       var result =  await _roleManager.CreateAsync(role);
        if (result.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });
            await _context.AddRangeAsync(permissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
            return Result.Success(response);
        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
}
