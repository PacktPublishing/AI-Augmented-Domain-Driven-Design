using BrewUp.Shared.Helpers;

namespace BrewUp.Sales.SharedKernel.Enums;

public class SalesOrderStatus(int id, string name) : Enumeration(id, name)
{
    public static SalesOrderStatus Accepted = new (1, nameof(Accepted).ToLowerInvariant());
    public static SalesOrderStatus WorkInProgress = new (2, nameof(WorkInProgress).ToLowerInvariant());
    public static SalesOrderStatus Completed = new (3, nameof(Completed).ToLowerInvariant());
    public static SalesOrderStatus Closed = new (4, nameof(Closed).ToLowerInvariant());
    public static SalesOrderStatus Rejected = new (5, nameof(Rejected).ToLowerInvariant());
    public static SalesOrderStatus Confirmed = new (6, nameof(Confirmed).ToLowerInvariant());

    public static IEnumerable<SalesOrderStatus> List() => [Accepted, WorkInProgress, Completed, Closed, Rejected, Confirmed];

    public static SalesOrderStatus FromName(string name)
    {
        var salesOrderStatus = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return salesOrderStatus ??
               throw new Exception($"Possible values for SalesOrderStatus: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static SalesOrderStatus From(int id)
    {
        var salesOrderStatus = List().SingleOrDefault(s => s.Id == id);

        return salesOrderStatus ??
               throw new Exception($"Possible values for SalesOrderStatus: {string.Join(",", List().Select(s => s.Name))}");
    }
}
