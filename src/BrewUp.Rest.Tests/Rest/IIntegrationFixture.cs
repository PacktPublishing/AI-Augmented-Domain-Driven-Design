namespace BrewUp.Rest.Tests.Rest;

public interface IIntegrationFixture
{
    TestClient GetClient();
    void ResetAll();
}