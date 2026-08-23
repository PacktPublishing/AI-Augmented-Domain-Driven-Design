using System.Linq.Expressions;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.MasterData.ReadModel.Queries;

internal sealed class SupplierQueries(IMongoClient mongoClient) : IQueries<Supplier>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("MasterData");
    
    public async Task<Result<Supplier>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<Supplier>(nameof(Supplier));
        var filter = Builders<Supplier>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Supplier>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Supplier>.Success(ConstructAggregate<Supplier>());
    }

    public async Task<Result<PagedResult<Supplier>>> GetByFilterAsync(Expression<Func<Supplier, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Supplier>(nameof(Supplier));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Supplier>>.Success(new PagedResult<Supplier>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}