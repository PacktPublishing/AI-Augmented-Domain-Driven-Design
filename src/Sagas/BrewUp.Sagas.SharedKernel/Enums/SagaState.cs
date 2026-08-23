using BrewUp.Shared.Helpers;

namespace BrewUp.Sagas.SharedKernel.Enums;

public sealed class SagaState(int id, string name) : Enumeration(id, name)
{
    public static SagaState Accepted = new (1, nameof(Accepted).ToLowerInvariant());
    public static SagaState Started = new (2, nameof(Started).ToLowerInvariant());
    public static SagaState Closed = new (3, nameof(Closed).ToLowerInvariant());
    public static SagaState Cancelled = new (4, nameof(Cancelled).ToLowerInvariant());
    public static SagaState Rejected = new (5, nameof(Rejected).ToLowerInvariant());

    public static IEnumerable<SagaState> List() => new[] { Accepted, Started, Closed, Cancelled, Rejected };

    public static SagaState FromName(string name)
    {
        var sagaState = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return sagaState ??
               throw new Exception($"Possible values for SagaState: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static SagaState From(int id)
    {
        var sagaState = List().SingleOrDefault(s => s.Id == id);

        return sagaState ??
               throw new Exception($"Possible values for SagaState: {string.Join(",", List().Select(s => s.Name))}");
    }
}