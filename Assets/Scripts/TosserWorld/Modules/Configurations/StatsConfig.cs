using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Stats Configuration", menuName = "Modules/Stats")]
    public class StatsConfig : ModuleConfiguration
    {
        public bool HasHealth = false;
        public bool HasStamina = false;

        public int MaxHealth = 100;
        public int MaxStamina = 100;
    }
}
