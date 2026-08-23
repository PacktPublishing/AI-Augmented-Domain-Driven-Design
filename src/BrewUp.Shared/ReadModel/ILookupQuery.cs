using System.Linq.Expressions;

namespace BrewUp.Shared.ReadModel;

public interface ILookupQuery 
{
    Task<T> GetByIdAsync<T>(string id, CancellationToken cancellation) where T : DtoBase;

    Task<PagedResult<T>> GetAllAsync<T>(Expression<Func<T, bool>>? query, int page, int pageSize,
        CancellationToken cancellationToken) where T : DtoBase;
}