using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.SharedKernel.Persistence;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Infrastructure;

internal sealed class MessagesReceivedService(IDashboardsRepository<MessagesReceived> repository,
    ILoggerFactory loggerFactory) : IMessagesReceivedService
{
    private readonly ILogger<MessagesReceivedService> _logger = loggerFactory.CreateLogger<MessagesReceivedService>();

    public Task<Result<MessagesReceived>> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task AddAsync(MessagesReceived entity, CancellationToken cancellationToken) =>
        repository.AddAsync(entity, cancellationToken);
}