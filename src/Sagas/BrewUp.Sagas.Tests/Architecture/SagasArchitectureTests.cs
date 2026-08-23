using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BrewUp.Sagas.Facade;
using BrewUp.Shared.Tests;
using NetArchTest.Rules;

namespace BrewUp.Sagas.Tests.Architecture;

[ExcludeFromCodeCoverage]
public class SagasArchitectureTests
{
    [Fact]
    public void Should_SagasArchitecture_BeCompliant()
    {
        var types = Types.InAssembly(typeof(SagasFacadeHelper).Assembly);
        
        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjects(true, ["Sagas"]);

        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
    
    [Fact]
    public void SagasProjects_Should_Having_Namespace_StartingWith_Sagas()
    {
        var modulePath = Path.Combine(VisualStudioProvider.TryGetSolutionDirectoryInfo().FullName, "Sagas");
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
        
        var moduleTypes = Types.InAssemblies(moduleAssemblies)
            .That()
            .DoNotHaveNameStartingWith("<>")
            .And()
            .AreNotNested()
            .GetTypes();
        
        var typesWithCorrectNamespace = Types.InAssemblies(moduleAssemblies)
            .That()
            .ResideInNamespaceStartingWith("BrewUp.Sagas")
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
                    $"with 'BrewUp.Sagas' but is in namespace '{type.Namespace}'");
        }
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