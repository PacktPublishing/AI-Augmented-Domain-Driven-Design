namespace BrewUp.Shared.Helpers;

public sealed class OrderState(int id, string name) : Enumeration(id, name)
{
    public static OrderState Created = new (1, nameof(Created).ToLowerInvariant());
    public static OrderState Sent = new (2, nameof(Sent).ToLowerInvariant());
    public static OrderState Confirmed = new (3, nameof(Confirmed).ToLowerInvariant());
    public static OrderState Completed = new (4, nameof(Completed).ToLowerInvariant());
    public static OrderState Cancelled = new (5, nameof(Cancelled).ToLowerInvariant());

    public static IEnumerable<OrderState> List() => new[] { Created, Sent, Confirmed, Completed, Cancelled };

    public static OrderState FromName(string name)
    {
        var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return state ??
               throw new Exception($"Possible values for OrderState: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static OrderState From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);

        return state ??
               throw new Exception($"Possible values for OrderState: {string.Join(",", List().Select(s => s.Name))}");
    }
}
