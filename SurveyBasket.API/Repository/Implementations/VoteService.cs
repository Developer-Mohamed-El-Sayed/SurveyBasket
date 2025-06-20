namespace SurveyBasket.API.Repository.Implementations;

public class VoteService(SurveyBasketDbContext context) : IVoteService
{
    private readonly SurveyBasketDbContext _context = context;

    public async Task<Result> CreateAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);
        if (hasVote)
            return Result.Failure(VoteErrors.DublicatedVote);
        var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        if (!pollIsExist)
            return Result.Failure(PollErrors.PollNotFound);
        var availableQuestion = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestion))
            return Result.Failure(VoteErrors.InvalidQuestions);
        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };
        await _context.Votes.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
