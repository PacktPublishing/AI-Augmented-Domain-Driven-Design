using System.Linq.Expressions;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.MasterData.ReadModel.Queries;

internal sealed class CustomerQueries(IMongoClient mongoClient) : IQueries<Customer>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("MasterData");

    public async Task<Result<Customer>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<Customer>(nameof(Customer));
        var filter = Builders<Customer>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Customer>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Customer>.Success(ConstructAggregate<Customer>());
    }

    public async Task<Result<PagedResult<Customer>>> GetByFilterAsync(Expression<Func<Customer, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Customer>(nameof(Customer));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Customer>>.Success(new PagedResult<Customer>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}