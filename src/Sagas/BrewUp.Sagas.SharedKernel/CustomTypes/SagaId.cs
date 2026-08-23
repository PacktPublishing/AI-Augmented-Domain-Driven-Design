using Muflone.Core;

namespace BrewUp.Sagas.SharedKernel.CustomTypes;

public sealed class SagaId(string value) : DomainId(value);