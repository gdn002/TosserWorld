using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Stacking Configuration", menuName = "Modules/Stacking")]
    public class StackingConfig : ModuleConfiguration
    {
        public int MaxAmount = 1;

        public bool IsStackable { get { return MaxAmount > 1; } }
    }
}
