namespace SurveyBasket.API.Repository.Implementation;

public class PollService(SurveyBasketDbContext context) : IPollService
{
    private readonly SurveyBasketDbContext _context = context;

    public async Task<Result<PollResponse>> CreateAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var response = request.Adapt<Poll>();
        await _context.AddAsync(response, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = response.Adapt<PollResponse>();
        return Result.Success(result);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(id, cancellationToken);
        if(response is null)
            return Result.Failure(PollErrors.PollNotFound);
        _context.Remove(response.Adapt<Poll>());
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
        if (response is null)
            return Result.Failure<PollResponse>(PollErrors.PollNotFound);
        var result = response.Adapt<PollResponse>();
        return Result.Success(result);
    }

    public async Task<Result> UpdateAsync(int id,PollRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(id, cancellationToken);
        if(response is null)
            return Result.Failure(PollErrors.PollNotFound);
        var result = response.Adapt<Poll>();
        _context.Update(result);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
