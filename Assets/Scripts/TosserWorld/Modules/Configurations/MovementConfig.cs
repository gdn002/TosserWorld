using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Movement Configuration", menuName = "Modules/Movement")]
    public class MovementConfig : ModuleConfiguration
    {
        public float SpeedLimit = 1;
        public bool OverridePhysics = false;

        public float FrameLimit { get { return SpeedLimit * Time.fixedDeltaTime; } }
    }
}
