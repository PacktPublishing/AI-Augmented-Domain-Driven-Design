using System.Linq.Expressions;
using BrewUp.Payment.ReadModel.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Payment.ReadModel.Queries;

public sealed class PaymentAuthorizationQueries(IMongoClient mongoClient)
    : IQueries<PaymentAuthorization>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Payment");

    public async Task<Result<PaymentAuthorization>> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var collection = _database.GetCollection<PaymentAuthorization>(
            nameof(PaymentAuthorization));
        var filter = Builders<PaymentAuthorization>.Filter.Eq("_id", id);
        var count = await collection
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (count == 0)
            return Result<PaymentAuthorization>.Error("Payment authorization not found.");

        using var cursor = await collection
            .FindAsync(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return Result<PaymentAuthorization>.Success(
            cursor.First(cancellationToken));
    }

    public async Task<Result<PagedResult<PaymentAuthorization>>> GetByFilterAsync(
        Expression<Func<PaymentAuthorization, bool>>? query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        page = Math.Max(page - 1, 0);
        var collection = _database.GetCollection<PaymentAuthorization>(
            nameof(PaymentAuthorization));
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

        return Result<PagedResult<PaymentAuthorization>>.Success(
            new PagedResult<PaymentAuthorization>(
                results,
                page,
                pageSize,
                count));
    }
}
