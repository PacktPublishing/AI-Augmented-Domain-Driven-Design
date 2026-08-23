namespace BrewUp.Shared.Tests;

public static class ModulesProjectUtils
{
    private static readonly string[] SolutionProjects = [
        "BrewUp.MasterData.Domain",
        "BrewUp.MasterData.Entities",
        "BrewUp.MasterData.Facade",
        "BrewUp.MasterData.Infrastructure",
        "BrewUp.MasterData.ReadModel", 
        "BrewUp.MasterData.SharedKernel",
        "BrewUp.MasterData.Tests",
        
        "BrewUp.Sales.Domain",
        "BrewUp.Sales.Entities",
        "BrewUp.Sales.Facade",
        "BrewUp.Sales.Infrastructure",
        "BrewUp.Sales.ReadModel", 
        "BrewUp.Sales.SharedKernel",
        "BrewUp.Sales.Tests",
        
        "BrewUp.Warehouse.Domain",
        "BrewUp.Warehouse.Entities",
        "BrewUp.Warehouse.Facade",
        "BrewUp.Warehouse.Infrastructure",
        "BrewUp.Warehouse.ReadModel", 
        "BrewUp.Warehouse.SharedKernel",
        "BrewUp.Warehouse.Tests",
        
        "BrewUp.Dashboards.Domain",
        "BrewUp.Dashboards.Entities",
        "BrewUp.Dashboards.Facade",
        "BrewUp.Dashboards.Infrastructure",
        "BrewUp.Dashboards.ReadModel", 
        "BrewUp.Dashboards.SharedKernel",
        "BrewUp.Dashboards.Tests",
        
        "BrewUp.Sagas.Domain",
        "BrewUp.Sagas.Entities",
        "BrewUp.Sagas.Facade",
        "BrewUp.Sagas.Infrastructure",
        "BrewUp.Sagas.ReadModel", 
        "BrewUp.Sagas.SharedKernel",
        "BrewUp.Sagas.Tests",
        
        "BrewUp.Purchases.Domain",
        "BrewUp.Purchases.Entities",
        "BrewUp.Purchases.Facade",
        "BrewUp.Purchases.Infrastructure",
        "BrewUp.Purchases.ReadModel", 
        "BrewUp.Purchases.SharedKernel",
        "BrewUp.Purchases.Tests"
    ];

    public static IEnumerable<string> GetModuleProjects(bool includeFacadeProjects, IEnumerable<string> excludeModules)
    {
        return SolutionProjects
            .Where(project =>
                (includeFacadeProjects || !project.EndsWith(".Facade")) &&
                !excludeModules.Any(module => project.StartsWith($"BrewUp.{module}.")));
    }
    
    public static IEnumerable<string> GetModuleProjectsWithoutDomain(string moduleName)
    {
        return SolutionProjects
            .Where(project =>
                project.StartsWith($"BrewUp.{moduleName}") &&
                !project.StartsWith($"BrewUp.{moduleName}.Domain") &&
                !project.StartsWith($"BrewUp.{moduleName}.SharedKernel"));
    }
}