using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Sales.ReadModel.Services;

public abstract class ServiceBase
{
    protected readonly IPersister Persister;
    protected readonly ILogger Logger;

    protected ServiceBase([FromKeyedServices("sales")] IPersister persister,
        ILoggerFactory loggerFactory)
    {
        Persister = persister;
        Logger = loggerFactory.CreateLogger(GetType());
    }
}