using Lena.Core;

namespace BrewUp.Shared.ReadModel;

public interface IPersister
{
    Task<Result<T>> GetByIdAsync<T>(string id, CancellationToken cancellationToken) where T : DtoBase;
    Task<Result<bool>> InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase;
    Task<Result<bool>> UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase;
    Task<Result<bool>> DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase;
}