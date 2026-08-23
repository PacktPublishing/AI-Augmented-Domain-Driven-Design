using CustomTypes = BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
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

        internal void ReserveItemStock(
            StockReservationId stockReservationId,
            SalesOrderId salesOrderId,
            CustomTypes.Quantity quantity,
            Guid correlationId)
        {
            ArgumentNullException.ThrowIfNull(stockReservationId);
            ArgumentNullException.ThrowIfNull(salesOrderId);
            ArgumentNullException.ThrowIfNull(quantity);

            if (quantity.UnitOfMeasure != _quantity.UnitOfMeasure)
                throw new ArgumentException("Unit of measure mismatch", nameof(quantity));

            if (quantity.Value > _quantity.Value)
                throw new InvalidOperationException("Cannot reserve more stock than available.");

            var remainingQuantity = new CustomTypes.Quantity(_quantity.Value - quantity.Value, _quantity.UnitOfMeasure);
            RaiseEvent(new ItemStockReserved(new AvailabilityId(Id.Value), stockReservationId, salesOrderId,
                quantity, remainingQuantity, correlationId));
        }

        private void Apply(ItemStockReserved @event)
        {
            _quantity = new Quantity(@event.RemainingQuantity.Value, @event.RemainingQuantity.UnitOfMeasure);
        }
    }
}
