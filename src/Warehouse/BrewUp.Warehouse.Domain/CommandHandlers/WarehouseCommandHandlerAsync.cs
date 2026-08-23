using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.Domain.CommandHandlers;

public abstract class WarehouseCommandHandlerAsync<TCommand>([FromKeyedServices("warehouse")] IPersister persister,
    ILoggerFactory loggerFactory) : ICommandHandlerAsync<TCommand> where TCommand : class, ICommand
{
    protected readonly IPersister Persister = persister ?? throw new ArgumentNullException(nameof(persister));
    
    public abstract Task HandleAsync(TCommand message, CancellationToken cancellationToken = new());
    
    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~WarehouseCommandHandlerAsync()
    {
        Dispose(false);
    }

    #endregion
}