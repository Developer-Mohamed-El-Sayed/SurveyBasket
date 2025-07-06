namespace SurveyBasket.API.Repository.Implementations;

public class NotificationService(SurveyBasketDbContext context,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender
    ) : INotificationService
{
    private readonly SurveyBasketDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task SendNewPollNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];
        if (pollId.HasValue)
        {
            var poll = await _context.Polls
               .SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished);
            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }
        //TODO : select Members Only 
        var users = await _userManager.Users
            .ToListAsync();
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholder = new Dictionary<string, string>
                {
                    {"{{Name}}", $"{user.FirstName} {user.LastName}" },
                    {"{{pollTitle}}", poll.Title },
                    {"{{endDate}}",poll.EndsAt.ToString()},
                    {"url",$"{origin}/polls/start/{poll.Id}" }
                };
                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholder);
                await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket : New Poll {poll.Title}", body);
            }
        }
    }
}
