using BrewUp.Dashboards.SharedKernel.Persistence;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.Domain.CommandHandlers;

public abstract class DashboardsCommandHandlerBaseAsync<TCommand>(IDashboardsRepository repository,
    ILoggerFactory loggerFactory) : ICommandHandlerAsync<TCommand> where TCommand : Command
{
    public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken = new());

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        int num = disposing ? 1 : 0;
    }
    
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
    }

    ~DashboardsCommandHandlerBaseAsync() => this.Dispose(false);    

    #endregion

}