namespace BrewUp.Shared.ReadModel;

public interface IReadModelPersister
{
    Task<T> GetByIdAsync<T>(string id, CancellationToken cancellation) where T : DtoBase;
    Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase;
    Task UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase;
    Task DeleteAsync<T>(string id, CancellationToken cancellationToken) where T : DtoBase;
}