using UnityEngine;
using TosserWorld.Entities;
using TosserWorld.Modules;

namespace TosserWorld.Utilities
{
    // Utility for "equipping" other entities
    public class EntityEquipment
    {
        private EquipmentSlot[] Slots;
        private ActionModule DefaultAction;

        public void Load(Entity owner)
        {
            Slots = owner.GetComponentsInChildren<EquipmentSlot>();

            foreach (var slot in Slots)
            {
                slot.Owner = owner;
            }
        }

        public EquipmentSlot this[int i]
        {
            get { return Slots[i]; }
        }

        public void DropAll()
        {
            foreach (var slot in Slots)
            {
                slot.DropEquipped();
            }
        }

        public int Length { get { return Slots.Length; } }
    }
}
