using System.Text;
using System.Text.Json;
using BrewUp.Payment.Facade;
using BrewUp.Payment.Facade.Endpoints;
using BrewUp.Payment.Facade.ExternalContracts;
using BrewUp.Payment.SharedKernel.DomainIds;
using Lena.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Payment.Tests.Facade;

public sealed class RecordAuthorizedCallbackBoundary
{
    [Fact]
    public async Task ProviderSuppliedOutcomeTraversesMappedCallback()
    {
        var builder = WebApplication.CreateBuilder();
        var facade = new RecordingFacade();
        builder.Services.AddSingleton<IPaymentFacade>(facade);
        await using var app = builder.Build();
        app.MapPaymentEndpoints();

        var authorizationId = Guid.CreateVersion7().ToString();
        var response = await InvokeMappedCallbackAsync(
            app,
            authorizationId,
            new ProviderAuthorizationJson("provider-auth-123"));

        Assert.Equal(StatusCodes.Status202Accepted, response.StatusCode);
        Assert.Equal(
            $"/v1/payment/authorizations/{authorizationId}",
            response.Headers.Location);
        Assert.Equal($"\"{authorizationId}\"", await ReadBodyAsync(response));
        Assert.Equal(authorizationId, facade.AuthorizationId?.Value);
        Assert.Equal("provider-auth-123", facade.ProviderReference);
        Assert.True(facade.CancellationToken.CanBeCanceled);
    }

    [Fact]
    public async Task FacadeFailureMapsToBadRequestThroughMappedCallback()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<IPaymentFacade>(
            new RecordingFacade(Result<string>.Error("provider callback rejected")));
        await using var app = builder.Build();
        app.MapPaymentEndpoints();

        var response = await InvokeMappedCallbackAsync(
            app,
            Guid.CreateVersion7().ToString(),
            new ProviderAuthorizationJson("provider-auth-rejected"));

        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Contains("provider callback rejected", await ReadBodyAsync(response));
    }

    private static async Task<HttpResponse> InvokeMappedCallbackAsync(
        WebApplication app,
        string authorizationId,
        ProviderAuthorizationJson body)
    {
        var endpoint = Assert.Single(
            ((IEndpointRouteBuilder)app).DataSources
                .SelectMany(source => source.Endpoints)
                .OfType<RouteEndpoint>(),
            candidate =>
                candidate.RoutePattern.RawText ==
                "/v1/payment/authorizations/{authorizationId}/authorized");
        Assert.Contains(
            "POST",
            endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods);

        var context = new DefaultHttpContext
        {
            RequestServices = app.Services
        };
        context.SetEndpoint(endpoint);
        using var requestAborted = new CancellationTokenSource();
        context.RequestAborted = requestAborted.Token;
        context.Request.Method = HttpMethods.Post;
        context.Request.RouteValues["authorizationId"] = authorizationId;
        var requestJson = JsonSerializer.Serialize(body);
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestJson));
        context.Request.ContentLength = context.Request.Body.Length;
        context.Request.ContentType = "application/json";
        context.Features.Set<IHttpRequestBodyDetectionFeature>(
            new RequestBodyDetectionFeature());
        context.Response.Body = new MemoryStream();

        await endpoint.RequestDelegate!(context);
        return context.Response;
    }

    private static async Task<string> ReadBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private sealed class RecordingFacade(
        Result<string>? result = null) : IPaymentFacade
    {
        private readonly Result<string>? _result = result;

        public PaymentAuthorizationId? AuthorizationId { get; private set; }
        public string? ProviderReference { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<Result<string>> RecordAuthorizedAsync(
            PaymentAuthorizationId authorizationId,
            string providerReference,
            CancellationToken cancellationToken)
        {
            AuthorizationId = authorizationId;
            ProviderReference = providerReference;
            CancellationToken = cancellationToken;
            return Task.FromResult(
                _result ?? Result<string>.Success(authorizationId.Value));
        }
    }

    private sealed class RequestBodyDetectionFeature : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => true;
    }
}
