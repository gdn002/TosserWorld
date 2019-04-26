using TosserWorld.Modules.Configurations;
using TosserWorld.UI;
using TosserWorld.Utilities;
using UnityEngine;

namespace TosserWorld.Modules
{
    /// <summary>
    /// Module for handling entity stats. This module also allows the entity to receive status effects.
    /// </summary>
    public class StatsModule : Module
    {
        private static GameObject StatBarPrefab;

        static StatsModule()
        {
            StatBarPrefab = Resources.Load<GameObject>("Prefabs/UI/World/StatBar");
        }

        public Stat Health;
        public Stat Stamina;

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            StatsConfig statsConfig = configuration as StatsConfig;

            Health = new Stat(statsConfig.MaxHealth, statsConfig.HasHealth);
            Stamina = new Stat(statsConfig.MaxStamina, statsConfig.HasStamina);

            GameObject healthBar = Object.Instantiate(StatBarPrefab);
            healthBar.GetComponent<UIStatBar>().TrackedStat = Health;
            healthBar.transform.SetParent(Owner.transform);
            healthBar.transform.localPosition = new Vector3(0, 0, -1);
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
