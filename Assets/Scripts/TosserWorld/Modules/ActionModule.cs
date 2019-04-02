using UnityEngine;
using TosserWorld.Modules.Configurations;
using TosserWorld.Entities;
using TosserWorld.Modules.ActionScripts;
using System.Collections;

namespace TosserWorld.Modules
{
    public enum ActionType
    {
        RunScript = 0,
        SpawnPrefab,
    }

    public enum ActionAnimation
    {
        NoAnimation = 0,
        Swing,
    }

    public class ActionModule : Module
    {
        public int RateOfFire { get; private set; }
        public bool AutoFire { get; private set; }
        public bool RunAndGun { get; private set; }

        public ActionAnimation ActionAnimation { get; private set; }
        public ActionType ActionType { get; private set; }
        public ActionScript ActionScript { get; private set; }
        public GameObject SpawnPrefab { get; private set; }

        private bool Ready = true;

        private int ActivationFrame = 0;
        private float ActivationDelay { get { return 0.017f * ActivationFrame; } }

        private float TimeBetweenShots { get { return (60f / RateOfFire); } }
        private float Timer = 0;


        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            ActionConfig actionConfig = configuration as ActionConfig;
            RateOfFire = actionConfig.RateOfFire;
            AutoFire = actionConfig.AutoFire;
            RunAndGun = actionConfig.RunAndGun;

            ActionAnimation = actionConfig.ActionAnimation;
            ActionType = actionConfig.ActionType;
            SpawnPrefab = actionConfig.SpawnPrefab;

            ActivationFrame = actionConfig.ActivationFrame();

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

                AnimateAction(actor);
                DoAction(actor);
            }
        }

        private void AnimateAction(Entity actor)
        {
            switch(ActionAnimation)
            {
                case ActionAnimation.NoAnimation:
                    return;
                case ActionAnimation.Swing:
                    actor.Animator.SetTrigger("Swing");
                    return;
            }
        }

        private void DoAction(Entity actor)
        {
            Timer = 0;
            Ready = false;

            switch (ActionType)
            {
                case ActionType.RunScript:
                    Owner.StartCoroutine(RunActionScript(actor));
                    break;
                case ActionType.SpawnPrefab:
                    Owner.StartCoroutine(RunSpawnPrefab(actor));
                    break;
            }
        }

        private bool CanFire(bool hold)
        {
            if (!Ready)
                return false;

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

        private IEnumerator RunActionScript(Entity actor)
        {
            yield return new WaitForSeconds(ActivationDelay);
            ActionScript.Run(actor);
            Ready = true;
        }

        private IEnumerator RunSpawnPrefab(Entity actor)
        {
            yield return new WaitForSeconds(ActivationDelay);
            Object.Instantiate(SpawnPrefab, actor.Position, actor.transform.rotation);
            Ready = true;
        }
    }
}
