using UnityEditor;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(InteractionConfig))]
    public class InteractionConfigEditor : Editor
    {
        private InteractionConfig Target { get { return target as InteractionConfig; } }

        public override void OnInspectorGUI()
        {
            Target.DefaultInteraction = (Interactions)EditorGUILayout.EnumPopup("Default Interaction: ", Target.DefaultInteraction);
            Target.DefaultDeadInteraction = (Interactions)EditorGUILayout.EnumPopup("Default Dead Interaction: ", Target.DefaultDeadInteraction);

            EditorUtility.SetDirty(target);
        }
    }
}
