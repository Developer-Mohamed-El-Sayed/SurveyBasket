namespace SurveyBasket.API.Repository.Implementation;

public class PollService(SurveyBasketDbContext context) : IPollService
{
    private readonly SurveyBasketDbContext _context = context;

    public async Task<PollResponse> CreateAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var response = request.Adapt<Poll>();
        await _context.AddAsync(response, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return response.Adapt<PollResponse>();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(id, cancellationToken);
        _context.Remove(response.Adapt<Poll>());
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls
            .AsNoTracking()
            .OrderBy(s => s.StartsAt)
            .ToListAsync(cancellationToken);
        return response.Adapt<IEnumerable<PollResponse>>();
    }

    public async Task<PollResponse> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls
            .OrderBy(s => s.StartsAt)
            .SingleOrDefaultAsync(pk => pk.Id == id,cancellationToken);
        return response.Adapt<PollResponse>();
    }

    public async Task UpdateAsync(int id,PollRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(id, cancellationToken);
        var responseToMainModel = new Poll
        {
            Id = id,
            Title = response.Title,
            Summary = response.Summary,
            IsPublished = response.IsPublished,
            StartsAt = response.StartsAt,
            EndsAt = response.EndsAt
        };
        //var responseToMainModel1 = response.Adapt<Poll>();
        _context.Update(responseToMainModel);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
