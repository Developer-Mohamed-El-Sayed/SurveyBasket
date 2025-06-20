namespace SurveyBasket.API.Repository.Interfaces;

public interface IVoteService
{
    Task<Result> CreateAsync(int pollId,string userId,VoteRequest request, CancellationToken cancellationToken = default);
}
