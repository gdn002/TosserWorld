using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entity.Type;

namespace Entity
{
    public class InventorySpace
    {
        public BasicEntity[] Inventory;

        public int Length { get { return Inventory.Length; } }

        public InventorySpace(int size)
        {
            Inventory = new BasicEntity[size];
        }

        public BasicEntity this[int index]
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
