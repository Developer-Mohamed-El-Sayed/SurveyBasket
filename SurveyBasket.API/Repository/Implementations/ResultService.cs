namespace SurveyBasket.API.Repository.Implementations;

public class ResultService(SurveyBasketDbContext context) : IResultService
{
    private readonly SurveyBasketDbContext _context = context;

    public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls
            .Where(x => x.Id == pollId)
            .Select(x => new PollVoteResponse(
                x.Title,
                x.Summary,
                x.Votes.Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content


            ))))))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
        return pollVotes is null
            ? Result.Failure<PollVoteResponse>(PollErrors.PollNotFound)
            : Result.Success(pollVotes);
    }
    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetPollVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
            .Select(g => new VotesPerDayResponse(g.Key.Date, g.Count()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return votesPerDay is null
            ? Result.Failure<IEnumerable<VotesPerDayResponse>>(VoteErrors.VoteNotFound)
            : Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }
    public async Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotePerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotePerQuestionResponse>>(PollErrors.PollNotFound);
        var votesPerQuestion = await _context.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .Select(x => new VotePerQuestionResponse(
                x.Question.Content,
                x.Question.VoteAnswers
                .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                .Select(g => new VotePerAnswersResponse(
                    g.Key.AnswerContent,
                    g.Count()
                ))
            )).AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<VotePerQuestionResponse>>(votesPerQuestion);
    }
}
