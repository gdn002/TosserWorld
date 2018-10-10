using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class InventorySpace
    {
        public EntityStack[] Inventory;

        public int Length { get { return Inventory.Length; } }

        public InventorySpace(int size)
        {
            Inventory = new EntityStack[size];
        }

        public EntityStack this[int index]
        {
            get
            {
                return Inventory[index];
            }
            set
            {
                Inventory[index] = value;
            }
        }
    }
}
