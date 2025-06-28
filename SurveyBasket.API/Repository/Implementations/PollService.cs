namespace SurveyBasket.API.Repository.Implementations;

public class PollService(SurveyBasketDbContext context,
    INotificationService notificationService
    ) : IPollService
{
    private readonly SurveyBasketDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<PollResponse>> CreateAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistTitle = await _context.Polls.AnyAsync(t => t.Title == request.Title, cancellationToken: cancellationToken);
        if (isExistTitle)
            return Result.Failure<PollResponse>(PollErrors.DublicatedTitle);
        var response = request.Adapt<Poll>();
        await _context.AddAsync(response, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = response.Adapt<PollResponse>();
        return Result.Success(result);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls.FindAsync(id, cancellationToken);
        if(response is null)
            return Result.Failure(PollErrors.PollNotFound);
        _context.Remove(response);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls
            .AsNoTracking()
            .OrderBy(s => s.StartsAt)
            .ToListAsync(cancellationToken);
        if (response is null)
            return Result.Failure<IEnumerable<PollResponse>>(PollErrors.PollNotFound);
        var result = response.Adapt<IEnumerable<PollResponse>>();
        return Result.Success(result);
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls
            .OrderBy(s => s.StartsAt)
            .SingleOrDefaultAsync(pk => pk.Id == id,cancellationToken);
        var result = response.Adapt<PollResponse>();
        return result is not null
            ? Result.Success(result)
            : Result.Failure<PollResponse>(PollErrors.PollNotFound);
        
    }

    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default) => 
       await _context.Polls
        .Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
        .AsNoTracking()
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);
    

    public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var response =  await _context.Polls.FindAsync(id,cancellationToken);
        if(response is null)
            return Result.Failure(PollErrors.PollNotFound);
        response.IsPublished = !response.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        if (response.IsPublished && response.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollNotification(response.Id));
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int id,PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistTitle = await _context.Polls.AnyAsync(t => t.Title == request.Title && t.Id != id, cancellationToken: cancellationToken);
        if (isExistTitle)
            return Result.Failure<PollResponse>(PollErrors.DublicatedTitle);
        var response = await _context.Polls.FindAsync(id, cancellationToken);
        if(response is null)
            return Result.Failure(PollErrors.PollNotFound);
        response.Title = request.Title;
        response.EndsAt = request.EndsAt;
        response.StartsAt = request.StartsAt;   
        response.Summary = request.Summary;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
