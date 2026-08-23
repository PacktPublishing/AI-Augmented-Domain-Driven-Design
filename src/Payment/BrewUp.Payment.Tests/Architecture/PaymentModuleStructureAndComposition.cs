using System.Reflection;
using System.Xml.Linq;

namespace BrewUp.Payment.Tests.Architecture;

public sealed class PaymentModuleStructureAndComposition
{
    private static readonly string[] PaymentProjects =
    [
        "Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj",
        "Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj",
        "Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj",
        "Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj",
        "Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj",
        "Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj"
    ];

    private static readonly string[] GovernedDependencyItemTypes =
    [
        "ProjectReference",
        "PackageReference",
        "FrameworkReference",
        "Reference"
    ];

    private static readonly string[] CrossContextForbiddenDependencies =
    [
        "BrewUp.Sales.Domain",
        "BrewUp.Sales.Infrastructure",
        "BrewUp.Warehouse.Domain",
        "BrewUp.Warehouse.Infrastructure"
    ];

    private static readonly string[] DomainForbiddenDependencies =
    [
        "BrewUp.Payment.ReadModel",
        "BrewUp.Payment.Infrastructure",
        "BrewUp.Payment.Facade",
        "BrewUp.Payment.Tests",
        "Microsoft.AspNetCore",
        "MongoDB",
        "RabbitMQ",
        "BrewUp.Sales.Domain",
        "BrewUp.Sales.Infrastructure",
        "BrewUp.Warehouse.Domain",
        "BrewUp.Warehouse.Infrastructure"
    ];

    [Theory]
    [InlineData("ProjectReference")]
    [InlineData("PackageReference")]
    [InlineData("FrameworkReference")]
    [InlineData("Reference")]
    public void DependencyInspectionIncludesEveryGovernedItemType(string itemType)
    {
        var project = XDocument.Parse(
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <ItemGroup>
                 <{itemType} Include="BrewUp.Payment.Infrastructure" />
               </ItemGroup>
             </Project>
             """);

        var dependency = Assert.Single(GetDependencyItems(project));

        Assert.Equal(itemType, dependency.ItemType);
        Assert.Equal("BrewUp.Payment.Infrastructure", dependency.Include);
    }

    [Theory]
    [InlineData("PackageReference", "Microsoft.AspNetCore.Mvc")]
    [InlineData("FrameworkReference", "Microsoft.AspNetCore.App")]
    [InlineData("Reference", "MongoDB.Driver")]
    [InlineData("PackageReference", "RabbitMQ.Client")]
    [InlineData("Reference", "BrewUp.Payment.Infrastructure")]
    [InlineData("ProjectReference", "../Sales/BrewUp.Sales.Domain/BrewUp.Sales.Domain.csproj")]
    public void DomainDependencyInspectionFindsForbiddenUnusedReferences(
        string itemType,
        string include)
    {
        var project = XDocument.Parse(
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <ItemGroup>
                 <{itemType} Include="{include}" />
               </ItemGroup>
             </Project>
             """);

        var dependency = Assert.Single(
            FindForbiddenDependencyItems(project, DomainForbiddenDependencies));

        Assert.Equal(itemType, dependency.ItemType);
        Assert.Equal(include, dependency.Include);
    }

    [Fact]
    public void ContractsAreOwnedByPaymentSharedKernel()
    {
        var sharedKernel = Assembly.Load("BrewUp.Payment.SharedKernel");
        string[] expectedTypes =
        [
            "BrewUp.Payment.SharedKernel.DomainIds.PaymentAuthorizationId",
            "BrewUp.Payment.SharedKernel.CustomTypes.SalesOrderReference",
            "BrewUp.Payment.SharedKernel.Messages.Commands.RequestPaymentAuthorization",
            "BrewUp.Payment.SharedKernel.Messages.Commands.RecordPaymentAuthorized",
            "BrewUp.Payment.SharedKernel.Messages.Events.PaymentAuthorizationRequested",
            "BrewUp.Payment.SharedKernel.Messages.Events.PaymentAuthorized",
            "BrewUp.Payment.SharedKernel.Messages.Events.PaymentAuthorizedIntegrationEvent"
        ];

        foreach (var expectedType in expectedTypes)
        {
            Assert.True(
                sharedKernel.GetType(expectedType) is not null,
                $"Expected Payment-owned contract '{expectedType}' in BrewUp.Payment.SharedKernel.");
        }
    }

    [Fact]
    public void ProjectDependencyItemsFollowPaymentLayerBoundaries()
    {
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj",
            [],
            ForbiddenDependenciesForPaymentLayers(
                "Domain", "ReadModel", "Infrastructure", "Facade", "Tests"));
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj",
            ["../BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj"],
            DomainForbiddenDependencies);
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj",
            [
                "../../BrewUp.Shared/BrewUp.Shared.csproj",
                "../BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj"
            ],
            ForbiddenDependenciesForPaymentLayers(
                "Domain", "Infrastructure", "Facade", "Tests"));
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj",
            [
                "../../BrewUp.Shared/BrewUp.Shared.csproj",
                "../BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj",
                "../BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj"
            ],
            ForbiddenDependenciesForPaymentLayers("ReadModel", "Facade", "Tests"));
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj",
            [
                "../BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj",
                "../BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj",
                "../BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj",
                "../BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj"
            ],
            ForbiddenDependenciesForPaymentLayers("Tests"));
        AssertProjectDependencies(
            "Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj",
            [
                "../../BrewUp.Shared/BrewUp.Shared.csproj",
                "../BrewUp.Payment.Domain/BrewUp.Payment.Domain.csproj",
                "../BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj",
                "../BrewUp.Payment.Infrastructure/BrewUp.Payment.Infrastructure.csproj",
                "../BrewUp.Payment.ReadModel/BrewUp.Payment.ReadModel.csproj",
                "../BrewUp.Payment.SharedKernel/BrewUp.Payment.SharedKernel.csproj"
            ],
            CrossContextForbiddenDependencies);
    }

    [Fact]
    public void DomainHasNoForbiddenLayerOrFrameworkDependencies()
    {
        var referencedAssemblies = Assembly.Load("BrewUp.Payment.Domain")
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        foreach (var forbiddenDependency in DomainForbiddenDependencies)
        {
            Assert.DoesNotContain(
                referencedAssemblies,
                reference => reference.StartsWith(forbiddenDependency, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void SolutionContainsAllSixPaymentProjectsInPaymentFolder()
    {
        var solution = XDocument.Load(Path.Combine(RepositoryRoot, "src", "BrewUp.slnx"));
        var paymentFolder = solution
            .Descendants("Folder")
            .SingleOrDefault(folder => (string?)folder.Attribute("Name") == "/50 Modules/Payment/");

        Assert.True(paymentFolder is not null, "Expected /50 Modules/Payment/ in src/BrewUp.slnx.");

        var projectPaths = paymentFolder!
            .Elements("Project")
            .Select(project => Normalize((string?)project.Attribute("Path") ?? string.Empty))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(PaymentProjects.Order(StringComparer.Ordinal), projectPaths);
    }

    [Fact]
    public void RestHostComposesPaymentThroughFacadeOnly()
    {
        var modulePath = Path.Combine(RepositoryRoot, "src", "BrewUp.Rest", "Module", "PaymentModule.cs");
        Assert.True(File.Exists(modulePath), "Expected PaymentModule in the REST composition root.");

        var module = File.ReadAllText(modulePath);
        Assert.Contains("class PaymentModule : IModule", module, StringComparison.Ordinal);
        Assert.Contains("AddPaymentFacade", module, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(module, "MapPaymentEndpoints"));

        var program = File.ReadAllText(Path.Combine(RepositoryRoot, "src", "BrewUp.Rest", "Program.cs"));
        Assert.Equal(1, CountOccurrences(program, "new PaymentModule()"));

        var restProject = XDocument.Load(
            Path.Combine(RepositoryRoot, "src", "BrewUp.Rest", "BrewUp.Rest.csproj"));
        var paymentDependencies = GetDependencyItems(restProject)
            .Where(dependency => IsPaymentDependency(dependency.Include))
            .ToArray();

        Assert.Collection(
            paymentDependencies,
            dependency =>
            {
                Assert.Equal("ProjectReference", dependency.ItemType);
                Assert.Equal(
                    "../Payment/BrewUp.Payment.Facade/BrewUp.Payment.Facade.csproj",
                    Normalize(dependency.Include));
            });
    }

    private static void AssertProjectDependencies(
        string projectPath,
        string[] expectedProjectReferences,
        string[] forbiddenDependencies)
    {
        var project = XDocument.Load(Path.Combine(RepositoryRoot, "src", projectPath));
        var projectReferences = GetDependencyItems(project)
            .Where(dependency => dependency.ItemType == "ProjectReference")
            .Select(dependency => Normalize(dependency.Include))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            expectedProjectReferences.Select(Normalize).Order(StringComparer.Ordinal),
            projectReferences);

        var forbiddenItems = FindForbiddenDependencyItems(project, forbiddenDependencies);
        Assert.True(
            forbiddenItems.Length == 0,
            $"{projectPath} contains forbidden dependency items: " +
            string.Join(", ", forbiddenItems.Select(
                dependency => $"{dependency.ItemType}={dependency.Include}")));
    }

    private static DependencyItem[] GetDependencyItems(XDocument project) =>
        project
            .Descendants()
            .Where(element => GovernedDependencyItemTypes.Contains(
                element.Name.LocalName,
                StringComparer.Ordinal))
            .Select(element => new DependencyItem(
                element.Name.LocalName,
                (string?)element.Attribute("Include") ?? string.Empty))
            .Where(dependency => !string.IsNullOrWhiteSpace(dependency.Include))
            .ToArray();

    private static DependencyItem[] FindForbiddenDependencyItems(
        XDocument project,
        IEnumerable<string> forbiddenDependencies) =>
        GetDependencyItems(project)
            .Where(dependency => forbiddenDependencies.Any(forbidden =>
                Normalize(dependency.Include).Contains(
                    forbidden,
                    StringComparison.OrdinalIgnoreCase)))
            .ToArray();

    private static string[] ForbiddenDependenciesForPaymentLayers(params string[] layers) =>
        layers
            .Select(layer => $"BrewUp.Payment.{layer}")
            .Concat(CrossContextForbiddenDependencies)
            .ToArray();

    private static bool IsPaymentDependency(string include)
    {
        var normalized = Normalize(include);
        return normalized.Contains("/Payment/", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("BrewUp.Payment.", StringComparison.OrdinalIgnoreCase);
    }

    private static string RepositoryRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null &&
                   !File.Exists(Path.Combine(directory.FullName, "src", "BrewUp.slnx")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName
                   ?? throw new DirectoryNotFoundException("Could not locate the BrewUp repository root.");
        }
    }

    private static string Normalize(string path) => path.Replace('\\', '/');

    private static int CountOccurrences(string value, string expected) =>
        value.Split(expected, StringSplitOptions.None).Length - 1;

    private sealed record DependencyItem(string ItemType, string Include);
}
