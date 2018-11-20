using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Entity.Component
{
    [CustomEditor(typeof(NPCStats))]
    public class NPCStatsEditor : Editor
    {
        private NPCStats Target { get { return target as NPCStats; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.Speed = EditorGUILayout.Slider("Speed: ", Target.Speed, 0, 10);
            EditorGUILayout.SelectableLabel("Speed/Frame: " + Target.FrameSpeed.ToString());
        }
    }
}
