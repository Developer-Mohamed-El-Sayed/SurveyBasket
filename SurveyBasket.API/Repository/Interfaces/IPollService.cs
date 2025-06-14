namespace SurveyBasket.API.Repository.Interfaces;

public interface IPollService
{
    Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PollResponse> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<PollResponse> CreateAsync(PollRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id,PollRequest request,CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
