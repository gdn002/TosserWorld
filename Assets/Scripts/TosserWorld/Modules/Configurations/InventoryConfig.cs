using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Inventory Configuration", menuName = "Module Configurations/Inventory")]
    public class InventoryConfig : ModuleConfiguration
    {
        public int Rows = 3;
        public int Cols = 3;

        public int SlotCount { get { return Rows * Cols; } }
    }
}
