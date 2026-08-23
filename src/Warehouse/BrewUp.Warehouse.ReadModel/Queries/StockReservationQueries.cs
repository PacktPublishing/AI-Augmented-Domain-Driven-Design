using System.Linq.Expressions;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel.Dtos;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Warehouse.ReadModel.Queries;

public sealed class StockReservationQueries(IMongoClient mongoClient)
    : IQueries<StockReservation>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Warehouse");

    public async Task<Result<StockReservation>> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var collection = _database.GetCollection<StockReservation>(
            nameof(StockReservation));
        var filter = Builders<StockReservation>.Filter.Eq("_id", id);
        var count = await collection
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (count == 0)
            return Result<StockReservation>.Error("Stock reservation not found.");

        using var cursor = await collection
            .FindAsync(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return Result<StockReservation>.Success(cursor.First(cancellationToken));
    }

    public async Task<Result<PagedResult<StockReservation>>> GetByFilterAsync(
        Expression<Func<StockReservation, bool>>? query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        page = Math.Max(page - 1, 0);
        var collection = _database.GetCollection<StockReservation>(
            nameof(StockReservation));
        var queryable = query is null
            ? collection.AsQueryable()
            : collection.AsQueryable().Where(query);
        var count = await queryable
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);
        var results = await queryable
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<PagedResult<StockReservation>>.Success(
            new PagedResult<StockReservation>(
                results,
                page,
                pageSize,
                count));
    }
}
