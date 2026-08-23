using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel.Dtos;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace BrewUp.Warehouse.ReadModel.Queries
{
    internal sealed class AvailabilityQueries(IMongoClient mongoClient) : IQueries<Availability>
    {
        private readonly IMongoDatabase _database = mongoClient.GetDatabase("Warehouse");

        public async Task<Result<Availability>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var collection = _database.GetCollection<Availability>(nameof(Availability));
            var filter = Builders<Availability>.Filter.Eq("_id", id);

            return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
                ? Result<Availability>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
                : Result<Availability>.Success(ConstructAggregate<Availability>());
        }

        public async Task<Result<PagedResult<Availability>>> GetByFilterAsync(Expression<Func<Availability, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (--page < 0)
                page = 0;

            var collection = _database.GetCollection<Availability>(nameof(Availability));
            var queryable = query != null
                ? collection.AsQueryable()
                    .Where(query)
                : collection.AsQueryable();

            var count = await queryable.CountAsync(cancellationToken: cancellationToken);
            var results = await queryable.Skip(page * pageSize).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            return Result<PagedResult<Availability>>.Success(new PagedResult<Availability>(results, page, pageSize, count));
        }

        private static TAggregate ConstructAggregate<TAggregate>()
        {
            return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
        }
    }
}
