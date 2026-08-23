using BrewUp.Sagas.SharedKernel.Messages.Commands;
using Lena.Core;

namespace BrewUp.Sagas.Domain.Orchestrators;

public interface ISalesOrderSagaOrchestrator
{
    Task<Result<string>> StartSagaAsync(StartSalesOrderSaga command, CancellationToken cancellationToken);
}