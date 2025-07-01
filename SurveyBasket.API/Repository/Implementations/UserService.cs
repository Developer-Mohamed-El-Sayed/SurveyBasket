namespace SurveyBasket.API.Repository.Implementations;

public class UserService(SurveyBasketDbContext context) : IUserService
{
    private readonly SurveyBasketDbContext _context = context;

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
}