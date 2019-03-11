using UnityEngine;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Interaction Configuration", menuName = "Modules/Interaction")]
    public class InteractionConfig : ModuleConfiguration
    {
        public Interactions Interaction = Interactions.NoInteraction;
    }
}
