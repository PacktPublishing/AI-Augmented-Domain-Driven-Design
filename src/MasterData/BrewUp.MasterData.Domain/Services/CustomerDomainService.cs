using BrewUp.MasterData.Domain.Helpers;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ReadModel;
using Lena.Asyncs;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.MasterData.Domain.Services;

internal sealed class CustomerDomainService([FromKeyedServices("masterdata")] IPersister persister,
    IIntegrationEventPublisher integrationEventPublisher) : ICustomerDomainService
{
    public async Task<Result<string>> RegisterCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva,
        Indirizzo indirizzo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Customer customer = Customer.Create(customerId, ragioneSociale, partitaIva,
            indirizzo.ToIndirizzoJson());

        // Railway-Oriented Programming pattern
        return (await (await persister.InsertAsync(customer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(customer.ToCustomerCreated(), cancellationToken)))
            .Match(
                _ => Result<string>.Success(customerId.Value),
                Result<string>.Error);
    }

    public async Task<Result<bool>> SetCustomerPropertiesAsync(CustomerPropertiesJson customerProperties,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await persister.GetByIdAsync<Customer>(customerProperties.CustomerId, cancellationToken);
        if (!customerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to update customer properties: {customerProperties.CustomerId}");
        
        customerResult.TryGetValue(out Customer customer);
        customer.UpdateConsumerLevel(customerProperties.ConsumerLevel);
        customer.UpdateBudgetLimit(customerProperties.BudgetLimit);
        customer.UpdateIsEnabled(customerProperties.IsEnabled);
        
        return (await persister.UpdateAsync(customer, cancellationToken))
            .Match(
                _ => Result<bool>.Success(true),
                Result<bool>.Error);
    }

    public async Task<Result<bool>> SaveCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva, Indirizzo indirizzo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await persister.GetByIdAsync<Customer>(customerId.Value, cancellationToken);
        if (!customerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to save customer: {customerId.Value}");
        
        customerResult.TryGetValue(out Customer customer);
        customer.UpdateRagioneSociale(ragioneSociale);
        customer.UpdatePartitaIva(partitaIva);
        customer.UpdateIndirizzo(indirizzo.ToIndirizzoJson());
        
        return (await (await persister.UpdateAsync(customer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(customer.ToCustomerUpdated(), cancellationToken)))
            .Match(
                _ => Result<bool>.Success(true),
                Result<bool>.Error);
    }

    public async Task<Result<bool>> DeleteCustomerAsync(CustomerId customerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await persister.GetByIdAsync<Customer>(customerId.Value, cancellationToken);
        if (!customerResult.IsSuccess) 
            return Result<bool>.Error($"Failed to delete customer: {customerId.Value}");
        
        customerResult.TryGetValue(out Customer customer);
        return (await (await persister.DeleteAsync(customer, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(customer.ToCustomerDeleted(), cancellationToken)))
            .Match(
                _ => Result<bool>.Success(true),
                Result<bool>.Error);
    }
}