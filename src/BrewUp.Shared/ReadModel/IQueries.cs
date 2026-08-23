using System.Linq.Expressions;
using Lena.Core;

namespace BrewUp.Shared.ReadModel;

public interface IQueries<T> where T : DtoBase
{
    Task<Result<T>> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<PagedResult<T>>> GetByFilterAsync(Expression<Func<T, bool>>? query, int page, int pageSize, CancellationToken cancellationToken);
}