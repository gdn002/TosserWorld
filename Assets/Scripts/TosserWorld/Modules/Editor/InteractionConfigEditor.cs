using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(InteractionConfig))]
    public class InteractionConfigEditor : Editor
    {
        private InteractionConfig Target { get { return target as InteractionConfig; } }

        public override void OnInspectorGUI()
        {
            Target.Interaction = (Interactions)EditorGUILayout.EnumPopup("Default Interaction: ", Target.Interaction);
            EditorUtility.SetDirty(target);
        }
    }
}
