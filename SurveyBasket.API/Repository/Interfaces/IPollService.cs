namespace SurveyBasket.API.Repository.Interfaces;

public interface IPollService
{
    Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> CreateAsync(PollRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id,PollRequest request,CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);
}
