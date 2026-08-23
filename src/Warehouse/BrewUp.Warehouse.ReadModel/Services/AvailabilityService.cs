using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Availability = BrewUp.Warehouse.ReadModel.Dtos.Availability;

namespace BrewUp.Warehouse.ReadModel.Services
{
    internal class AvailabilityService([FromKeyedServices("warehouse")] IPersister persister,
    IQueries<Availability> queries,
    ILoggerFactory loggerFactory)
    : ServiceBase(persister, loggerFactory), IAvailabilityService
    {
        public async Task<Result<AvailabilityJson>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await queries.GetByIdAsync(id, cancellationToken);

            if (!queryResult.IsSuccess) 
                return Result<AvailabilityJson>.Error("Availability not found");
            
            queryResult.TryGetValue(out Availability availabilityDto);
            return Result<AvailabilityJson>.Success(availabilityDto.ToJson());
        }

        public async Task<Result<bool>> AddAvailabilityAsync(AvailabilityId availabilityId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity,
            CancellationToken cancellationToken)
        {
            var dto = Availability.Create(availabilityId, warehouseId, beerId, quantity);

            return await Persister.InsertAsync(dto, cancellationToken);
        }

        public async Task<Result<string>> AddItemStockAsync(AvailabilityId availabilityId,
            Quantity quantity,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var persisterResult = await Persister.GetByIdAsync<Availability>(availabilityId.Value, cancellationToken);
            if (!persisterResult.IsSuccess)
                return Result<string>.Error("Error retrieving warehouse availability");

            persisterResult.TryGetValue(out Availability availabilityDto);
            availabilityDto.UpdateQuantity(quantity);

            var updateResult = await Persister.UpdateAsync(availabilityDto, cancellationToken);
            return updateResult.Match(
                _ => Result<string>.Success(availabilityId.Value),
                _ => Result<string>.Error("Error updating warehouse availability"));
        }

        public async Task<Result<AvailabilityJson>> GetAvailabilityByWarehouseIdAndBeerIdAsync(WarehouseId warehouseId, BeerId beerId, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await queries.GetByFilterAsync(a => a.WarehouseId == warehouseId.Value 
                && a.BeerId == beerId.Value, 1, int.MaxValue, cancellationToken);

            if (!queryResult.IsSuccess)
                return Result<AvailabilityJson>.Error("Availability not found");
            
            queryResult.TryGetValue(out PagedResult<Availability> availabilityDto);
            return availabilityDto.Results.Any() 
                ? Result<AvailabilityJson>.Success(availabilityDto.Results.First().ToJson())
                : Result<AvailabilityJson>.Error("No availability found");
        }

        public async Task<Result<AvailabilityJson>> GetAvailabilityByBeerIdAsync(BeerId beerId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await queries.GetByFilterAsync(a => a.BeerId == beerId.Value, 0, 1, cancellationToken);

            if (!queryResult.IsSuccess) 
                return Result<AvailabilityJson>.Error("Availability not found");
            
            queryResult.TryGetValue(out PagedResult<Availability> availabilityDto);
            return availabilityDto.Results.Any() 
                ? Result<AvailabilityJson>.Success(availabilityDto.Results.First().ToJson())
                : Result<AvailabilityJson>.Error("No availability found");
        }

        public Task<Result<ReorderThreshold>> GetReorderThresholdByBeerIdAsync(BeerId beerId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            ReorderThreshold reorderThresold = new (beerId, new ThresholdQuantity(300, "Bottle"));
            return Task.FromResult(Result<ReorderThreshold>.Success(reorderThresold));
        }
    }
}
