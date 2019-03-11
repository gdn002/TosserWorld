using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Physics Configuration", menuName = "Modules/Physics")]
    public class PhysicsConfig : ModuleConfiguration
    {
        // Drag value for vertical movement
        public float AirDrag = 1;

        // The object's gravity scale
        public float GravityScale = 1;

        // The object's bounciness
        public float Bounciness = 0.1f;

        // The object's friction modifier
        public float Friction = 1;

        public bool EnableOnCollisions = true;
    }
}
