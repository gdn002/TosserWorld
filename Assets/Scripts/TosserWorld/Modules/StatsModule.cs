using UnityEngine;

using TosserWorld.Entities;
using TosserWorld.Utilities;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Stats Configuration", menuName = "Modules/Stats")]
    public class StatsConfig : ModuleConfiguration
    {
        public bool HasHealth = false;
        public bool HasStamina = false;

        public int MaxHealth = 100;
        public int MaxStamina = 100;
    }

    public class StatsModule : Module
    {
        public Stat Health;
        public Stat Stamina;

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            StatsConfig statsConfig = configuration as StatsConfig;

            Health = new Stat(statsConfig.MaxHealth, statsConfig.HasHealth);
            Stamina = new Stat(statsConfig.MaxStamina, statsConfig.HasStamina);
        }

        public override void Update()
        {
            // Automatic stat updates

            if (Health.Current <= 0)
            {
                Owner.Destroy();
            }
        }
    }
}
