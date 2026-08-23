using System.Diagnostics.CodeAnalysis;
using BrewUp.Infrastructure;
using BrewUp.Sales.Domain;
using BrewUp.Sales.Facade;
using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.ReadModel.Services;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BrewUp.Rest.Tests.Modules;

[ExcludeFromCodeCoverage]
public class SalesModuleDependencyInjectionTests
{
    [Fact]
    public void All_Services_Should_Be_Registered()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(new NullLoggerFactory());
        IConfigurationManager configurationManager = new ConfigurationManager();
        configurationManager.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
        
        services.AddInfrastructure(new NullLoggerFactory(), configurationManager);
        services.AddSalesFacade(configurationManager);
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        
        var salesFacade = serviceProvider.GetService<ISalesFacade>();
        var salesDomainService = serviceProvider.GetService<ISalesDomainService>();
        var salesOrderService = serviceProvider.GetService<ISalesOrderService>();
        var customerService = serviceProvider.GetService<ICustomerService>();
        var salesPersister = serviceProvider.GetRequiredKeyedService(typeof(IPersister), "sales");
        var salesOrderQueries = serviceProvider.GetService<IQueries<SalesOrder>>();

        Assert.NotNull(salesFacade);
        Assert.NotNull(salesDomainService);
        Assert.NotNull(salesOrderService);
        Assert.NotNull(customerService);
        Assert.NotNull(salesPersister);
        Assert.NotNull(salesOrderQueries);
    }
}