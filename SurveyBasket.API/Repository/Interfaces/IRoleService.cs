namespace SurveyBasket.API.Repository.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<RoleDetailResponse>> GetAsync(string id);
    Task<Result<RoleDetailResponse>> CreateAsync(RoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string id, RoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string id);
}
