using UnityEngine;
using TosserWorld.Modules.Configurations;
using TosserWorld.Entities;

namespace TosserWorld.Modules
{
    public class ActionModule : Module
    {
        public GameObject ActionPrefab;

        public int RateOfFire = 60;

        public bool AutoFire = false;
        public bool RunAndGun = false;

        private float TimeBetweenShots { get { return (60f / RateOfFire); } }
        private float Timer = 0;

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            ActionConfig actionConfig = configuration as ActionConfig;
            ActionPrefab = actionConfig.ActionPrefab;
            RateOfFire = actionConfig.RateOfFire;
            AutoFire = actionConfig.AutoFire;
            RunAndGun = actionConfig.RunAndGun;
        }

        public void Activate(Entity actor, bool hold)
        {
            if (CanFire(hold))
            {
                if (!RunAndGun)
                {
                    // If run and gun is disallowed, activating this action stops movement
                    if (Owner.Movement != null)
                        Owner.Movement.Stop();
                }

                Timer = 0;
                GameObject action = Object.Instantiate(ActionPrefab, actor.Position, actor.transform.rotation);
            }
        }

        private bool CanFire(bool hold)
        {
            if (AutoFire)
            {
                // If the action allows auto fire, first shots are uncapped
                if (!hold)
                {
                    return true;
                }

                // Auto fire shots are capped by fire rate
                return Timer >= TimeBetweenShots;
            }
            
            // If the action disallows auto fire, ignore auto fire shots
            if (hold)
            {
                return false;
            }

            // Cap first shots by fire rate
            return Timer >= TimeBetweenShots;
        }

        public override void Update()
        {
            Timer += Time.deltaTime;
        }
    }
}
