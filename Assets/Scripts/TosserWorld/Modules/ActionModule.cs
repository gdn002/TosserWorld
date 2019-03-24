using UnityEngine;
using TosserWorld.Modules.Configurations;
using TosserWorld.Entities;
using TosserWorld.Modules.ActionScripts;

namespace TosserWorld.Modules
{
    public enum ActionType
    {
        RunScript = 0,
        SpawnPrefab,
    }

    public class ActionModule : Module
    {
        public int RateOfFire { get; private set; }
        public bool AutoFire { get; private set; }
        public bool RunAndGun { get; private set; }

        public ActionType ActionType { get; private set; }
        public ActionScript ActionScript { get; private set; }
        public GameObject SpawnPrefab { get; private set; }

        private float TimeBetweenShots { get { return (60f / RateOfFire); } }
        private float Timer = 0;

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            ActionConfig actionConfig = configuration as ActionConfig;
            RateOfFire = actionConfig.RateOfFire;
            AutoFire = actionConfig.AutoFire;
            RunAndGun = actionConfig.RunAndGun;

            ActionType = actionConfig.ActionType;
            SpawnPrefab = actionConfig.SpawnPrefab;

            ActionScript = ActionScriptSelector.InstantiateScript(actionConfig.SelectedScript);
            ActionScript.Initialize(Owner);
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

                DoAction(actor);
            }
        }

        public void DoAction(Entity actor)
        {
            Timer = 0;

            switch (ActionType)
            {
                case ActionType.RunScript:
                    ActionScript.Run(actor);
                    break;
                case ActionType.SpawnPrefab:
                    Object.Instantiate(SpawnPrefab, actor.Position, actor.transform.rotation);
                    break;
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
