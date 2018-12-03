using UnityEditor;
using TosserWorld.Modules.BrainScripts;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor : Editor
    {
        private Brain Target { get { return target as Brain; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.ActiveBrain = EditorGUILayout.ObjectField("Active Brain: ", Target.ActiveBrain, typeof(BrainScript), false) as BrainScript;
            Target.AwarenessRadius = EditorGUILayout.FloatField("Awareness Radius: ", Target.AwarenessRadius);

            EditorUtility.SetDirty(target);
        }
    }
}
