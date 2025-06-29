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
                select new UserResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    user.IsDisabled,
                    roles.Select(x => x.Name!).ToList()
                )
              )
        .ToListAsync(cancellationToken);
        


}
