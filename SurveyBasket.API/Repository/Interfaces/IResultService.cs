namespace SurveyBasket.API.Repository.Interfaces;

public interface IResultService
{
    Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
}
