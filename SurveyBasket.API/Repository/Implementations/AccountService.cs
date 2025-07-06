namespace SurveyBasket.API.Repository.Implementations;

public class AccountService(UserManager<ApplicationUser> userManager,
    SurveyBasketDbContext context
    ) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SurveyBasketDbContext _context = context;

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.InvalidUser);
        var result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {
        var user = await _context.Users
                        .Where(pk => pk.Id == userId)
                        .ProjectToType<UserProfileResponse>()
                        .AsNoTracking()
                        .SingleAsync();
        return Result.Success(user);
    }

    public async Task<Result> LogOutAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.InvalidUser);
        // and fix that ....
        var result = await _userManager.RemoveLoginAsync(user, "", "");
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateUserProfileAsync(string userId, UpdateUserProfileRequest request)
    {
        await _userManager.Users
            .Where(pk => pk.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.FirstName, request.FirstName)
                .SetProperty(x => x.LastName, request.LastName)
            );
        return Result.Success();
    }
}
