using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Action Configuration", menuName = "Module Configurations/Action")]
    public class ActionConfig : ModuleConfiguration
    {
        public int RateOfFire = 60;
        public bool AutoFire = false;
        public bool RunAndGun = false;

        public ActionType ActionType = ActionType.RunScript;
        public int SelectedScript = 0;
        public GameObject SpawnPrefab;

        public float TimeBetweenShots { get { return (60f / RateOfFire); } }
    }
}
