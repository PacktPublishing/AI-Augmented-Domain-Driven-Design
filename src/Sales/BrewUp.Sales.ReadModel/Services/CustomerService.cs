using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.ReadModel.Helpers;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Sales.ReadModel.Services;

internal sealed class CustomerService([FromKeyedServices("sales")] IPersister persister,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory), ICustomerService
{
    public async Task<Result<bool>> AddCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva,
        BeerConsumerLevel consumerLevel, Indirizzo indirizzo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customer = Customer.Create(customerId, ragioneSociale, partitaIva, consumerLevel, indirizzo.ToIndirizzoJson());
        var insertResult = await Persister.InsertAsync(customer, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating customer: {Error}", error);
                return Result<bool>.Error($"Error creating customer: {error}");
            });
    }

    public async Task<Result<bool>> UpdateCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva,
        BeerConsumerLevel consumerLevel, Indirizzo indirizzo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await Persister.GetByIdAsync<Customer>(customerId.Value, cancellationToken);
        if (!customerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to save customer: {customerId.Value}");
        
        customerResult.TryGetValue(out Customer customer);
        customer.UpdateRagioneSociale(ragioneSociale);
        customer.UpdatePartitaIva(partitaIva);
        customer.UpdateIndirizzo(indirizzo.ToIndirizzoJson());
        
        return await Persister.UpdateAsync(customer, cancellationToken);
    }

    public async Task<Result<bool>> DeleteCustomerAsync(CustomerId customerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await Persister.GetByIdAsync<Customer>(customerId.Value, cancellationToken);
        if (!customerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to delete customer: {customerId.Value}");
        
        customerResult.TryGetValue(out Customer customer);
        
        return await Persister.DeleteAsync(customer, cancellationToken);
    }
}