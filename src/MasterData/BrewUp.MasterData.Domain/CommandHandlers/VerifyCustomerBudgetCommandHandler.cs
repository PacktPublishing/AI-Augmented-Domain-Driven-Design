using BrewUp.MasterData.Entities.Dtos;
using BrewUp.MasterData.SharedKernel.Messages.Commands;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muflone;

namespace BrewUp.MasterData.Domain.CommandHandlers;

public sealed class VerifyCustomerBudgetCommandHandler([FromKeyedServices("masterdata")] IPersister persister, 
    IEventBus eventBus,
    ILoggerFactory loggerFactory) : MasterDataCommandHandlerAsync<VerifyCustomerBudget>(persister, loggerFactory)
{
    public override async Task HandleAsync(VerifyCustomerBudget command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var customerResult = await Persister.GetByIdAsync<Customer>(command.AggregateId.Value, cancellationToken);
        if (!customerResult.IsSuccess) 
            return;
        
        var correlationId = command.MessageId;
        customerResult.TryGetValue(out Customer customer);

        if (!customer.IsEnabled)
        {
            CustomerBudgetUnVerified unVerifiedIntegrationEvent = new(new CustomerId(customer.Id), correlationId, "Customer is disabled");
            await eventBus.PublishAsync(unVerifiedIntegrationEvent,  cancellationToken);
            
            return;
        }
        
        if (customer.BudgetLimit < command.AmountToCheck.Value)
        {
            CustomerBudgetUnVerified unVerifiedIntegrationEvent = new(new CustomerId(customer.Id), correlationId, "Customer budget limit exceeded");
            await eventBus.PublishAsync(unVerifiedIntegrationEvent,  cancellationToken);
            
            return;
        }
        
        CustomerBudgetVerified integrationEvent = new(new CustomerId(customer.Id), correlationId,
            customer.ToJson());
        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}