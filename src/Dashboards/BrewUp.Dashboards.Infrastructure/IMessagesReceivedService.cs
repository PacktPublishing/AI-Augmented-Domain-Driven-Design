using BrewUp.Dashboards.Entities.Dtos;
using Lena.Core;

namespace BrewUp.Dashboards.Infrastructure;

public interface IMessagesReceivedService
{
    Task<Result<MessagesReceived>> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(MessagesReceived entity, CancellationToken cancellationToken);
}