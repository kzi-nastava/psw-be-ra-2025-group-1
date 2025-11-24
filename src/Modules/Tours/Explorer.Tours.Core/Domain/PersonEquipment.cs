using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
    public  class PersonEquipment : Entity
    {
        public long PersonId { get; init; }
        public long EquipmentId { get; init; }
        public int Quantity { get; init; }
        public PersonEquipment(long personId, long equipmentId, int quantity)
        {
            if (personId <= 0) throw new ArgumentException("PersonId is invalid");
            if (equipmentId <= 0) throw new ArgumentException("EquipmentId is invalid");
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
            PersonId = personId;
            EquipmentId = equipmentId;
            Quantity = quantity;
        }
    }

