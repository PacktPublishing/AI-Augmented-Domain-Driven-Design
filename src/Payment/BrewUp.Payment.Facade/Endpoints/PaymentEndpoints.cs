using BrewUp.Payment.Facade.ExternalContracts;
using BrewUp.Payment.SharedKernel.DomainIds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Payment.Facade.Endpoints;

public static class PaymentEndpoints
{
    public static WebApplication MapPaymentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/payment")
            .WithTags("Payment");

        group.MapPost(
                "/authorizations/{authorizationId}/authorized",
                HandleAuthorizedAsync)
            .Produces<string>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("RecordPaymentAuthorized");

        return app;
    }

    private static async Task<IResult> HandleAuthorizedAsync(
        string authorizationId,
        ProviderAuthorizationJson body,
        IPaymentFacade facade,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await facade
            .RecordAuthorizedAsync(
                new PaymentAuthorizationId(authorizationId),
                body.ProviderReference,
                cancellationToken)
            .ConfigureAwait(false);

        return result.Match<IResult>(
            success => Results.Accepted(
                $"/v1/payment/authorizations/{authorizationId}",
                success),
            Results.BadRequest);
    }
}
