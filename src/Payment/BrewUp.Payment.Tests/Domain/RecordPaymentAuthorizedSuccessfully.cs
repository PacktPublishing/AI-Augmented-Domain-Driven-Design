using BrewUp.Payment.Domain.CommandHandlers;
using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using BrewUp.Payment.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Payment.Tests.Domain;

public sealed class RecordPaymentAuthorizedSuccessfully
    : CommandSpecification<RecordPaymentAuthorized>
{
    private readonly PaymentAuthorizationId _id = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderReference _order = new(Guid.CreateVersion7().ToString());
    private readonly Guid _requestCorrelationId = Guid.CreateVersion7();
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new PaymentAuthorizationRequested(_id, _order, _requestCorrelationId);
    }

    protected override RecordPaymentAuthorized When() =>
        new(_id, "provider-auth-123", _correlationId);

    protected override ICommandHandlerAsync<RecordPaymentAuthorized> OnHandler() =>
        new RecordPaymentAuthorizedCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new PaymentAuthorized(_id, "provider-auth-123", _correlationId);
    }
}
