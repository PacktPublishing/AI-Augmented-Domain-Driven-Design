using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BrewUp.MasterData.Facade;
using BrewUp.Shared.Tests;
using NetArchTest.Rules;

namespace BrewUp.MasterData.Tests.Architecture;

[ExcludeFromCodeCoverage]
public class MasterDataArchitectureTests
{
    [Fact]
    public void Should_MasterDataArchitecture_BeCompliant()
    {
        var types = Types.InAssembly(typeof(MasterDataHelper).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjects(true, ["MasterData"]);
        
        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
    
    [Fact]
    public void MasterDataProjects_Should_Having_Namespace_StartingWith_MasterData()
    {
        var modulePath = Path.Combine(VisualStudioProvider.TryGetSolutionDirectoryInfo().FullName, "MasterData");
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
            .ResideInNamespaceStartingWith("BrewUp.MasterData")
            .And()
            .AreNotNested()
            .GetTypes();
        
        // Find types with incorrect namespace (difference between the two sets)
        var modulesTypeArray = moduleTypes as Type[] ?? moduleTypes.ToArray();
        var typesWithIncorrectNamespace = modulesTypeArray.Except(typesWithCorrectNamespace).ToList();

        foreach (var type in typesWithIncorrectNamespace)
        {
            if (type.Namespace != null)
                Assert.Fail(
                    $"Namespace violation detected: {type.FullName} in assembly {type.Assembly.GetName().Name} should start " +
                    $"with 'BrewUp.MasterData' but is in namespace '{type.Namespace}'");
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