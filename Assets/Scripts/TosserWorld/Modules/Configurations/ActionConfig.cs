using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Action Configuration", menuName = "Module Configurations/Action")]
    public class ActionConfig : ModuleConfiguration
    {
        public int RateOfFire = 60;
        public bool AutoFire = false;
        public bool RunAndGun = false;
        public bool ConsumesItem = false;

        public ActionAnimation ActionAnimation = ActionAnimation.NoAnimation;
        public ActionType ActionType = ActionType.RunScript;
        public int SelectedScript = 0;
        public GameObject SpawnPrefab;

        public float TimeBetweenShots { get { return (60f / RateOfFire); } }

        public int ActivationFrame()
        {
            // These should be set to the proper activation frame for the animation itself
            switch (ActionAnimation)
            {
                case ActionAnimation.NoAnimation:
                    return 0;
                case ActionAnimation.Swing:
                    return 21;
            }

            return 0;
        }

    }
}
