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
    public async Task<Result> UpdateAsync(string id,RoleRequest request, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        var roleIsExist = await _roleManager.Roles
            .AnyAsync(x => x.Name == request.Name && id != x.Id, cancellationToken: cancellationToken);

        if (roleIsExist)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DublicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            var currentPermissions = await _context.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync(cancellationToken);

            var newPermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = id
                });

            var removedPermisssions = currentPermissions.Except(request.Permissions);
            await _context.RoleClaims
                .Where(x => x.RoleId == id && removedPermisssions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync(cancellationToken);

            await _context.AddRangeAsync(newPermissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);
        role.IsDeleted = !role.IsDeleted;
        var result = await _roleManager.UpdateAsync(role);
        if(result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
    }
}
