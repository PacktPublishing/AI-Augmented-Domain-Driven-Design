using CustomTypes = BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Core;

namespace BrewUp.Warehouse.Domain.Entities
{
    public class Availability : AggregateRoot
    {
        internal WarehouseId _warehouseId;
        internal BeerId _beerId;
        internal Quantity _quantity;

        protected Availability() { }

        internal static Availability Create(AvailabilityId aggregateId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity)
        {
            return new Availability(aggregateId, warehouseId, beerId, quantity);
        }

        private Availability(AvailabilityId aggregateId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity)
        {
            RaiseEvent(new AvailabilityCreated(aggregateId,
                warehouseId,
                beerId,
                new Shared.CustomTypes.Quantity(quantity.Value, quantity.UnitOfMeasure)));
        }

        private void Apply(AvailabilityCreated @event)
        {
            Id = @event.AggregateId;
            _warehouseId = @event.WarehouseId;
            _beerId = @event.BeerId;
            _quantity = new Quantity(@event.Quantity.Value, @event.Quantity.UnitOfMeasure);
        }

        internal void AddItemStock(Quantity quantity)
        {
            if (quantity == null) throw new ArgumentNullException(nameof(quantity));

            if (_quantity == null)
            {
                _quantity = new Quantity(quantity.Value, quantity.UnitOfMeasure);
            }
            else
            {
                //TODO: consider implementing unit conversion if units of measure differ
                if (quantity.UnitOfMeasure != _quantity.UnitOfMeasure) throw new ArgumentException("Unit of measure mismatch", nameof(quantity));

                _quantity = new Quantity(_quantity.Value + quantity.Value, _quantity.UnitOfMeasure);
            }

            RaiseEvent(new ItemStockAdded(new AvailabilityId(Id.Value), new CustomTypes.Quantity(_quantity.Value, _quantity.UnitOfMeasure)));
        }

        private void Apply(ItemStockAdded @event)
        {
            // This replaces the quantity instead of adding it.
            _quantity = new Quantity(@event.Quantity.Value, @event.Quantity.UnitOfMeasure);
        }

        /// <summary>
        /// Reserve stock for a sales order. Partial reservation is allowed (OQ-2):
        /// Warehouse decides the reservable subset. Sales never performs this decision (BC-005/BC-006).
        /// </summary>
        internal void ReserveStock(IEnumerable<CustomTypes.ItemRequested> requestedRows,
            string salesOrderId, Guid correlationId)
        {
            var stockReservationId = new StockReservationId(Guid.CreateVersion7().ToString());
            var warehouseId = new WarehouseId(_warehouseId.Value);

            // Find rows that match this availability's beer
            var matchingRows = requestedRows
                .Where(r => r.BeerId.Value == _beerId.Value)
                .ToList();

            if (matchingRows.Count == 0 || _quantity.Value <= 0)
            {
                RaiseEvent(new StockReservationRejected(warehouseId, correlationId, salesOrderId,
                    "No stock available for the requested beers"));
                return;
            }

            // Partial reservation: reserve up to available quantity
            var totalRequested = matchingRows.Sum(r => r.QuantityOrdered.Value);
            var reservableQuantity = Math.Min(_quantity.Value, totalRequested);

            var reservedRows = matchingRows
                .Select(r => new CustomTypes.ItemRequested(
                    r.BeerId,
                    new CustomTypes.Quantity(Math.Min(r.QuantityOrdered.Value, reservableQuantity), r.QuantityOrdered.UnitOfMeasure),
                    r.QuantityAvailable))
                .ToList();

            RaiseEvent(new StockReserved(warehouseId, correlationId, stockReservationId, salesOrderId, reservedRows));
        }

        private void Apply(StockReserved @event)
        {
            // Stock reduced by reservation (domain decision: Warehouse owns stock mutation, BC-007)
            // Note: quantity reduction tracking for reservations can be extended here
        }

        private void Apply(StockReservationRejected @event)
        {
            // No state change on rejection
        }
    }
}