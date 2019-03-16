using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Brain Configuration", menuName = "Module Configurations/Brain")]
    public class BrainConfig : ModuleConfiguration
    {
        public int SelectedBrainScript = 0;
        public float AwarenessRadius = 5;
    }
}
