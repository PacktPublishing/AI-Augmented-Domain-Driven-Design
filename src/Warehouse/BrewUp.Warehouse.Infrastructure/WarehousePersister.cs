using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BrewUp.Warehouse.Infrastructure;

public sealed class WarehousePersister(IMongoClient mongoClient,
    ILoggerFactory loggerFactory) : IPersister
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Warehouse");
    private readonly ILogger _logger = loggerFactory.CreateLogger<WarehousePersister>();
    
    public async Task<Result<T>> GetByIdAsync<T>(string id, CancellationToken cancellationToken) where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        var type = typeof(T).Name;
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
                ? Result<T>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
                : Result<T>.Success(ConstructAggregate<T>());
        }
        catch (Exception e)
        {
            _logger.LogError("Insert: Error saving DTO: {Type}, Message: {EMessage}, StackTrace: {EStackTrace}", type,
                e.Message, e.StackTrace);
            return Result<T>.Error($"Insert: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
        }
    }

    public async Task<Result<bool>> InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        var type = typeof(T).Name;
        try
        {
            var collection = _database.GetCollection<T>(type);
            await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Insert: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
            return Result<bool>.Error($"Insert: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
        }
    }

    public async Task<Result<bool>> UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        var type = typeof(T).Name;
        try
        {
            var collection = _database.GetCollection<T>(type);
            await collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Update: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
            return Result<bool>.Error(
                $"Update: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
        }
    }

    public async Task<Result<bool>> DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : DtoBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        var type = typeof(T).Name;
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            await collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Delete: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
            return Result<bool>.Error(
                $"Delete: Error saving DTO: {type}, Message: {e.Message}, StackTrace: {e.StackTrace}");
        }
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}