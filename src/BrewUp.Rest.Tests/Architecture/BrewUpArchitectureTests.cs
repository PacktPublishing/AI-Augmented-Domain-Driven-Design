using System.Diagnostics.CodeAnalysis;
using BrewUp.Rest.Module;
using BrewUp.Shared.Tests;
using NetArchTest.Rules;
namespace BrewUp.Rest.Tests.Architecture;

[ExcludeFromCodeCoverage]
public class BrewUpArchitectureTests
{
    [Fact]
    public void Should_BrewUpArchitecture_BeCompliant()
    {
        var types = Types.InAssembly(typeof(IModule).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjects(false, []);
        
        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
}