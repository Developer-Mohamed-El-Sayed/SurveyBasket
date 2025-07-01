namespace SurveyBasket.API.Repository.Implementations;

public class UserService(SurveyBasketDbContext context, IRoleService roleService, UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly SurveyBasketDbContext _context = context;
    private readonly IRoleService _roleService = roleService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await(  from user in _context.Users
                join userRole in _context.UserRoles
                on user.Id equals userRole.UserId
                join role in _context.Roles
                on userRole.RoleId equals role.Id into roles
                where !roles.Any(x => x.Name == DefaultRoles.Member)
                select new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.IsDisabled,
                    Roles = roles.Select(x => x.Name).ToList()
                }
              )
        .GroupBy(u => new {u.Id,u.FirstName,u.LastName,u.Email,u.IsDisabled})
        .Select(x => new UserResponse(
            x.Key.Id,
            x.Key.FirstName,
            x.Key.LastName,
            x.Key.Email,
            x.Key.IsDisabled,
            x.SelectMany(r => r.Roles)
        ))
        .ToListAsync(cancellationToken);
    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.InvalidUser);
        var userRoles = await _userManager.GetRolesAsync(user);
        var response = (user,userRoles).Adapt<UserResponse>();
        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request,CancellationToken cancellationToken = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if(emailIsExist)
            return Result.Failure<UserResponse>(UserErrors.DublicatedEmail);
        var allowedRoles = await _roleService.GetAllAsync(cancellationToken);
        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(RoleErrors.RoleNotFound);

        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        if(result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);
            var response =(user,request.Roles).Adapt<UserResponse>();
            return Result.Success(response);
        }
        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request,CancellationToken cancellationToken = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);
        if (emailIsExist)
            return Result.Failure<UserResponse>(UserErrors.DublicatedEmail);
        var allowedRoles = await _roleService.GetAllAsync(cancellationToken);
        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(RoleErrors.RoleNotFound);
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.InvalidUser);
        user = request.Adapt(user);


        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _context.UserRoles
                .Where(x => x.UserId == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            await _userManager.AddToRolesAsync(user,request.Roles);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.InvalidUser);
        user.IsDisabled = !user.IsDisabled;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code,error.Description, StatusCodes.Status400BadRequest));
    }
}