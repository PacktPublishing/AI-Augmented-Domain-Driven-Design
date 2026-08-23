using BrewUp.Shared.Helpers;

namespace BrewUp.Warehouse.SharedKernel.Enums;

public sealed class ShipmentState(int id, string name) : Enumeration(id, name)
{
    public static ShipmentState PendingPreparation = new (1, nameof(PendingPreparation).ToLowerInvariant());
    public static ShipmentState PickingInProgress = new (2, nameof(PickingInProgress).ToLowerInvariant());
    public static ShipmentState Packed = new (3, nameof(Packed).ToLowerInvariant());
    public static ShipmentState ReadyForDispatch = new (4, nameof(ReadyForDispatch).ToLowerInvariant());

    public static IEnumerable<ShipmentState> List() => new[] { PendingPreparation, PickingInProgress, Packed, ReadyForDispatch };

    public static ShipmentState FromName(string name)
    {
        var shipmentState = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return shipmentState ??
               throw new Exception($"Possible values for ShipmentState: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static ShipmentState From(int id)
    {
        var shipmentState = List().SingleOrDefault(s => s.Id == id);

        return shipmentState ??
               throw new Exception($"Possible values for ShipmentState: {string.Join(",", List().Select(s => s.Name))}");
    }
}