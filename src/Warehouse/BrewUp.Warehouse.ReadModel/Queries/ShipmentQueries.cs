using System.Linq.Expressions;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel.Dtos;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Warehouse.ReadModel.Queries;

internal sealed class ShipmentQueries(IMongoClient mongoClient) : IQueries<Shipment>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Warehouse");
    
    public async Task<Result<Shipment>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<Shipment>(nameof(Shipment));
        var filter = Builders<Shipment>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Shipment>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Shipment>.Success(ConstructAggregate<Shipment>());
    }

    public async Task<Result<PagedResult<Shipment>>> GetByFilterAsync(Expression<Func<Shipment, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Shipment>(nameof(Shipment));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Shipment>>.Success(new PagedResult<Shipment>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}