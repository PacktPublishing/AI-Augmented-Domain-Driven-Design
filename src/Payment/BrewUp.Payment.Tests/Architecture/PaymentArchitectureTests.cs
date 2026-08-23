using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BrewUp.Payment.Domain;
using BrewUp.Payment.Facade;
using BrewUp.Shared.Tests;
using NetArchTest.Rules;

namespace BrewUp.Payment.Tests.Architecture;

[ExcludeFromCodeCoverage]
public class PaymentArchitectureTests
{
    [Fact]
    public void Should_PaymentArchitecture_BeCompliant()
    {
        var types = Types.InAssembly(typeof(PaymentFacadeHelper).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjects(true, ["Payment"]);

        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Outer_Layers()
    {
        var types = Types.InAssembly(typeof(PaymentDomainHelper).Assembly);

        var forbiddenAssemblies = ModulesProjectUtils.GetModuleProjectsWithoutDomain("Payment");

        var result = types
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenAssemblies.ToArray())
            .GetResult()
            .IsSuccessful;

        Assert.True(result);
    }
}
