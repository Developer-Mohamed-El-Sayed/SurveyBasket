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
}
