using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BrewUp.Shared.Tests;
using BrewUp.Warehouse.Domain;
using BrewUp.Warehouse.Facade;
using NetArchTest.Rules;

namespace BrewUp.Warehouse.Tests.Architecture;

[ExcludeFromCodeCoverage]
public class WarehouseArchitectureTests
{
    [Fact]
    public void Should_WarehouseArchitecture_BeCompliant()
    {
        var types = Types.InAssembly(typeof(WarehouseFacadeHelper).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjects(true, ["Warehouse"]);
        
        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
    
    [Fact]
    public void WarehouseProjects_Should_Having_Namespace_StartingWith_Warehouse()
    {
        var modulePath = Path.Combine(VisualStudioProvider.TryGetSolutionDirectoryInfo().FullName, "Warehouse");
        var subFolders = Directory.GetDirectories(modulePath);

        var netVersion = Environment.Version;

        var moduleAssemblies = (from folder in subFolders
            let binFolder = Path.Join(folder, "bin", "Debug", $"net{netVersion.Major}.{netVersion.Minor}")
            where Directory.Exists(binFolder)
            let files = Directory.GetFiles(binFolder)
            let folderArray = folder.Split(Path.DirectorySeparatorChar)
            select files.FirstOrDefault(f => f.EndsWith($"{folderArray[^1]}.dll"))
            into assemblyFilename
            where assemblyFilename != null && !assemblyFilename.Contains("Test")
            select Assembly.LoadFile(assemblyFilename)).ToList();
        
        moduleAssemblies = moduleAssemblies.Where(a => !a.GetName().Name!.Contains("McpServer")).ToList();
        
        var moduleTypes = Types.InAssemblies(moduleAssemblies)
            .That()
            .DoNotHaveNameStartingWith("<>")
            .And()
            .AreNotNested()
            .GetTypes();
        
        var typesWithCorrectNamespace = Types.InAssemblies(moduleAssemblies)
            .That()
            .ResideInNamespaceStartingWith("BrewUp.Warehouse")
            .And()
            .AreNotNested()
            .GetTypes();
        
        // Find types with incorrect namespace (difference between the two sets)
        var moduleTypeArray = moduleTypes as Type[] ?? moduleTypes.ToArray();
        var typesWithIncorrectNamespace = moduleTypeArray.Except(typesWithCorrectNamespace).ToList();

        foreach (var type in typesWithIncorrectNamespace)
        {
            if (type.Namespace != null)
                Assert.Fail(
                    $"Namespace violation detected: {type.FullName} in assembly {type.Assembly.GetName().Name} should start " +
                    $"with 'BrewUp.Warehouse' but is in namespace '{type.Namespace}'");
        }
    }
    
    [Fact]
    public void Domain_Should_Not_Depend_On_Outer_Layers()
    {
        var types = Types.InAssembly(typeof(DomainHelper).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjectsWithoutDomain("Warehouse");
        
        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
    
    private static class VisualStudioProvider
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.slnx").Any())
            {
                directory = directory.Parent;
            }
            return directory!
                   ?? throw new DirectoryNotFoundException("Solution directory not found. Make sure to run this test from a solution folder.");
        }
    }
}