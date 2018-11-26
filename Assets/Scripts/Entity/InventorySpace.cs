namespace TosserWorld
{
    public class InventorySpace
    {
        public Entity[] Inventory;

        public int Length { get { return Inventory.Length; } }

        public InventorySpace(int size)
        {
            Inventory = new Entity[size];
        }

        public Entity this[int index]
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
