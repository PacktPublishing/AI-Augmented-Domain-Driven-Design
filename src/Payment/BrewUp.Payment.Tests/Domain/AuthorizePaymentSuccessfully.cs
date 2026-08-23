using BrewUp.Payment.Domain.CommandHandlers;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Payment.Tests.Domain;

public sealed class AuthorizePaymentSuccessfully : CommandSpecification<AuthorizePayment>
{
    private readonly PaymentAuthorizationId _paymentId = new(Guid.CreateVersion7().ToString());
    private readonly string _salesOrderId = Guid.CreateVersion7().ToString();
    private readonly Price _amount = new(100m, "EUR");
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield break;
    }

    protected override AuthorizePayment When() =>
        new(_paymentId, _correlationId, _salesOrderId, _amount);

    protected override ICommandHandlerAsync<AuthorizePayment> OnHandler() =>
        new AuthorizePaymentCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new PaymentAuthorized(_paymentId, _correlationId, _salesOrderId);
    }
}
