using TosserWorld.Entities;
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

        private GameObject HealthBar;

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


            // UI updates

            if (PlayerEntity.Player.DistanceTo(Owner) < 5)
            {
                if (HealthBar == null)
                {
                    HealthBar = Object.Instantiate(StatBarPrefab);
                    HealthBar.GetComponent<UIStatBar>().TrackedStat = Health;
                    HealthBar.transform.SetParent(Owner.transform);
                    HealthBar.transform.localPosition = new Vector3(0, 0, -1);
                }
            }
            else if (HealthBar != null)
            {
                Object.Destroy(HealthBar);
                HealthBar = null;
            }
        }
    }
}
