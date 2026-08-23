using Lena.Core;
using Muflone.Messages.Events;

namespace BrewUp.MasterData.Domain.Services;

public interface IIntegrationEventPublisher
{
    Task<Result<bool>> PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
