using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Dashboards.SharedKernel.Persistence;

public interface IDashboardsRepository
{}

public interface IDashboardsRepository<T> : IDashboardsRepository where T : DtoBase
{
    Task<Result<T>> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(T entity, CancellationToken cancellationToken);
}