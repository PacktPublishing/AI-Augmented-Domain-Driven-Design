using System.Diagnostics.CodeAnalysis;
using BrewUp.Infrastructure;
using BrewUp.MasterData.Domain.Services;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.MasterData.Facade;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BrewUp.Rest.Tests.Modules;

[ExcludeFromCodeCoverage]
public class MasterDataModuleDependencyInjectionTests
{
    [Fact]
    public void All_Services_Should_Be_Registered()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(new NullLoggerFactory());
        IConfigurationManager configurationManager = new ConfigurationManager();
        configurationManager.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
        
        services.AddInfrastructure(new NullLoggerFactory(), configurationManager);
        services.AddMasterDataFacade();
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        var masterDataFacade = serviceProvider.GetService<IMasterDataCustomerFacade>();
        var masterDataDomainService = serviceProvider.GetService<ICustomerDomainService>();
        var masterDataPersister = serviceProvider.GetRequiredKeyedService(typeof(IPersister), "masterdata");
        var customerQueries = serviceProvider.GetService<IQueries<Customer>>();

        Assert.NotNull(masterDataFacade);
        Assert.NotNull(masterDataDomainService);
        Assert.NotNull(masterDataPersister);
        Assert.NotNull(customerQueries);
    }
}