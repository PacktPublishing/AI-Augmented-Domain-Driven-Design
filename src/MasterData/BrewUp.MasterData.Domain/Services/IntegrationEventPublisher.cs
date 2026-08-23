using Lena.Core;
using Muflone;
using Muflone.Messages.Events;

namespace BrewUp.MasterData.Domain.Services;

internal sealed class IntegrationEventPublisher(IEventBus eventBus) : IIntegrationEventPublisher
{
    public async Task<Result<bool>> PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        try
        {
            await eventBus.PublishAsync(integrationEvent, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Error($"Failed to send integration event: {ex.Message}");
        }
    }
}
