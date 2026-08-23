using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BrewUp.Payment.Infrastructure;

public sealed class PaymentPersister(
    IMongoClient mongoClient,
    ILoggerFactory loggerFactory) : IPersister
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Payment");
    private readonly ILogger<PaymentPersister> _logger =
        loggerFactory.CreateLogger<PaymentPersister>();

    public async Task<Result<T>> GetByIdAsync<T>(
        string id,
        CancellationToken cancellationToken)
        where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            var count = await collection
                .CountDocumentsAsync(filter, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (count == 0)
                return Result<T>.Error($"{typeof(T).Name} not found.");

            using var cursor = await collection
                .FindAsync(filter, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return Result<T>.Success(cursor.First(cancellationToken));
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error retrieving {ProjectionType}.",
                typeof(T).Name);
            return Result<T>.Error(exception);
        }
    }

    public async Task<Result<bool>> InsertAsync<T>(
        T entity,
        CancellationToken cancellationToken)
        where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection
                .InsertOneAsync(entity, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error inserting {ProjectionType}.",
                typeof(T).Name);
            return Result<bool>.Error(exception);
        }
    }

    public async Task<Result<bool>> UpdateAsync<T>(
        T entity,
        CancellationToken cancellationToken)
        where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var replaceResult = await collection
                .ReplaceOneAsync(
                    projection => projection.Id == entity.Id,
                    entity,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!replaceResult.IsAcknowledged || replaceResult.MatchedCount == 0)
            {
                return Result<bool>.Error(
                    $"{typeof(T).Name} with ID {entity.Id} was not replaced.");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error updating {ProjectionType}.",
                typeof(T).Name);
            return Result<bool>.Error(exception);
        }
    }

    public async Task<Result<bool>> DeleteAsync<T>(
        T entity,
        CancellationToken cancellationToken)
        where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            await collection
                .FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error deleting {ProjectionType}.",
                typeof(T).Name);
            return Result<bool>.Error(exception);
        }
    }
}
