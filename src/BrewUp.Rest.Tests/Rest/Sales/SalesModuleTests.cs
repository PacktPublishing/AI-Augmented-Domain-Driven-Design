using System.Net;

namespace BrewUp.Rest.Tests.Rest.Sales;

[Collection("Integration Fixture")]
public sealed class SalesModuleTests(IntegrationFixture integrationFixture) : IDisposable
{
     [Fact]
     public async Task Get_Sales()
     {
         var result = await integrationFixture.Client.GetAsync("/v1/sales");

         Assert.Equal(HttpStatusCode.OK, result.StatusCode);

         Assert.NotEqual("[]", await result.Content.ReadAsStringAsync());
     }
     
     #region Dispose

     public void Dispose()
     {
         integrationFixture.ResetAll();
     }

     #endregion
}