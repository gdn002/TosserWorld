using TosserWorld.Modules.Configurations;
using TosserWorld.Utilities;

namespace TosserWorld.Modules
{
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
                Owner.Kill();
            }
        }
    }
}
