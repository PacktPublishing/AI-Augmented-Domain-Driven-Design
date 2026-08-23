using System.Diagnostics;
using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.Facade.Acl;
using BrewUp.Warehouse.Facade.EventHandlers;
using BrewUp.Warehouse.ReadModel.EventHandlers;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using NetArchTest.Rules;

namespace BrewUp.Warehouse.Tests.Architecture;

public sealed class WarehouseReservationContractsStayInSharedKernel
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void ReservationContractsAndBehaviorRemainInOwnedLayers()
    {
        var sharedKernel = typeof(StockReservationId).Assembly;
        var contractTypes = new[]
        {
            typeof(StockReservationId),
            typeof(ReserveStock),
            typeof(StockReservationRequestedIntegrationEvent),
            typeof(StockReserved),
            typeof(StockReservationFailed),
            typeof(StockReservedIntegrationEvent),
            typeof(StockReservationFailedIntegrationEvent)
        };

        Assert.All(
            contractTypes,
            type => Assert.Same(sharedKernel, type.Assembly));
        Assert.Equal(
            "BrewUp.Warehouse.SharedKernel",
            sharedKernel.GetName().Name);
        Assert.Equal(
            "BrewUp.Warehouse.Domain",
            typeof(StockReservation).Assembly.GetName().Name);
        Assert.Equal(
            "BrewUp.Warehouse.ReadModel",
            typeof(StockReservedEventHandler).Assembly.GetName().Name);
        Assert.Equal(
            "BrewUp.Warehouse.Facade",
            typeof(StockReservationRequestedIntegrationEventHandler)
                .Assembly.GetName().Name);
        Assert.Equal(
            "BrewUp.Warehouse.Facade",
            typeof(StockReservedIntegrationEventPublisher).Assembly.GetName().Name);

        var domainBoundary = Types
            .InAssembly(typeof(StockReservation).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "BrewUp.Sales.Domain",
                "BrewUp.Sales.Infrastructure",
                "BrewUp.Payment.Domain",
                "BrewUp.Payment.Infrastructure",
                "BrewUp.Sagas.Domain",
                "BrewUp.Sagas.Infrastructure")
            .GetResult();
        Assert.True(domainBoundary.IsSuccessful);
    }

    [Fact]
    public void ExistingWarehouseCompositionFilesRemainUnchanged()
    {
        var unchangedFiles = new[]
        {
            "src/BrewUp.Rest/Module/WarehouseModule.cs",
            "src/Warehouse/BrewUp.Warehouse.Facade/Endpoints/WarehouseEndpoints.cs",
            "src/Warehouse/BrewUp.Warehouse.Facade/Acl/RequestBeerAvailablityRaisedEventHandler.cs",
            "src/Warehouse/BrewUp.Warehouse.Facade/Acl/SalesOrderCreatedIntegrationEventHandler.cs",
            "src/Warehouse/BrewUp.Warehouse.Infrastructure/InfrastructureHelper.cs"
        };

        var changed = RunGit(
            ["diff", "--name-only", "8ca284c00a08bcb1a8375369f341d830941280f6", "--", .. unchangedFiles]);

        Assert.True(
            string.IsNullOrWhiteSpace(changed),
            $"Preserved Warehouse composition changed:{Environment.NewLine}{changed}");
    }

    private static string RunGit(IReadOnlyList<string> arguments)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = RepositoryRoot
        };
        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, error);
        return output;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, ".git")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Git repository root not found.");
    }
}
