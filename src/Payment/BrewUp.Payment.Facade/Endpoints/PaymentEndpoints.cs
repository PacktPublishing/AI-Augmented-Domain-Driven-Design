using Microsoft.AspNetCore.Routing;

namespace BrewUp.Payment.Facade.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}
