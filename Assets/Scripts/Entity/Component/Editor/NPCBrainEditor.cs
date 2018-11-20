using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Entity.Component.Brains;

namespace Entity.Component
{
    [CustomEditor(typeof(NPCBrain))]
    public class NPCBrainEditor : Editor
    {
        private NPCBrain Target { get { return target as NPCBrain; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.ActiveBrain = EditorGUILayout.ObjectField("Active Brain: ", Target.ActiveBrain, typeof(Brain), false) as Brain;
            Target.AwarenessRadius = EditorGUILayout.FloatField("Awareness Radius: ", Target.AwarenessRadius);
        }
    }
}
