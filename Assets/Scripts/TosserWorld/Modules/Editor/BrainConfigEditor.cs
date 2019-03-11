using UnityEditor;
using TosserWorld.Modules.BrainScripts;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(BrainConfig))]
    public class BrainConfigEditor : Editor
    {
        private BrainConfig Target { get { return target as BrainConfig; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            Target.SelectedBrainScript = EditorGUILayout.Popup("Active Brain: ", Target.SelectedBrainScript, BrainScriptSelector.AllNames());
            Target.AwarenessRadius = EditorGUILayout.FloatField("Awareness Radius: ", Target.AwarenessRadius);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Target);
            }
        }
    }
}
