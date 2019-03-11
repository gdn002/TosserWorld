using UnityEditor;
using TosserWorld.Modules.BrainScripts;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(BrainConfig))]
    public class BrainConfigEditor : Editor
    {
        private BrainConfig Target { get { return target as BrainConfig; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SelectedBrainScript = EditorGUILayout.Popup("Active Brain: ", Target.SelectedBrainScript, BrainScriptSelector.AllNames());
            Target.AwarenessRadius = EditorGUILayout.FloatField("Awareness Radius: ", Target.AwarenessRadius);

            EditorUtility.SetDirty(target);
        }
    }
}
