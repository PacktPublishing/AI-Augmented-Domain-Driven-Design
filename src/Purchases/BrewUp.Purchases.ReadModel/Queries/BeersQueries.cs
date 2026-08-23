using System.Linq.Expressions;
using BrewUp.Purchases.ReadModel.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Purchases.ReadModel.Queries;

internal sealed class BeersQueries(IMongoClient mongoClient) : IQueries<Beer>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Sales");

    public async Task<Result<Beer>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<Beer>(nameof(Beer));
        var filter = Builders<Beer>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Beer>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Beer>.Success(ConstructAggregate<Beer>());
    }

    public async Task<Result<PagedResult<Beer>>> GetByFilterAsync(Expression<Func<Beer, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Beer>(nameof(Beer));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Beer>>.Success(new PagedResult<Beer>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}