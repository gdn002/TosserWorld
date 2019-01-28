using UnityEditor;
using TosserWorld.Modules.BrainScripts;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(BrainModule))]
    public class BrainModuleEditor : Editor
    {
        private BrainModule Target { get { return target as BrainModule; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SelectedBrainScript = EditorGUILayout.Popup("Active Brain: ", Target.SelectedBrainScript, BrainScriptSelector.AllNames());
            Target.AwarenessRadius = EditorGUILayout.FloatField("Awareness Radius: ", Target.AwarenessRadius);

            EditorUtility.SetDirty(target);
        }
    }
}
