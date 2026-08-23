using BrewUp.Rest.Module;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace BrewUp.Rest.Tests.Rest;

public sealed class IntegrationFixture : IIntegrationFixture, IDisposable
{
    public readonly TestClient Client;
    
    public IntegrationFixture()
    {
        var app = new MonitoringApplication();

        var oneClientToRuleThemAll = app.CreateClient();
        oneClientToRuleThemAll.BaseAddress = new Uri("https://localhost:5003");
        Client = new TestClient(oneClientToRuleThemAll);
    }
    
    public TestClient GetClient()
    {
        return Client;
    }

    public void ResetAll()
    {
        Client.ResetHeaders();
    }
    
    private class MonitoringApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Program is the real composition root; this keeps fixture expectations aligned with module discovery.
            return base.CreateHost(builder);
        }
    }
    
    #region Dispose

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
    
    ~IntegrationFixture()
    {
        Client.Dispose();
    }

    #endregion
}