using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
    public  class PersonEquipment : Entity
    {
        public long PersonId { get; init; }
        public long EquipmentId { get; init; }
        public PersonEquipment(long personId, long equipmentId)
        {
            if (personId == 0) throw new ArgumentException("PersonId is invalid");
            if (equipmentId == 0) throw new ArgumentException("EquipmentId is invalid");
            PersonId = personId;
            EquipmentId = equipmentId;
        }
    }

