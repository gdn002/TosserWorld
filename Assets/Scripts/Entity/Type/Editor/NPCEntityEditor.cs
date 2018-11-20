using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Entity.Component;

namespace Entity.Type
{
    [CustomEditor(typeof(NPCEntity))]
    public class NPCEntityEditor : BasicEntityEditor
    {
        private bool ShowNPCEntity = true;

        private NPCEntity Target { get { return target as NPCEntity; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowNPCEntity = EditorGUILayout.Foldout(ShowNPCEntity, "NPC Entity");
            if (ShowNPCEntity)
            {
                Target.Brain = EditorGUILayout.ObjectField("Brain: ", Target.Brain, typeof(NPCBrain), false) as NPCBrain;
                Target.Stats = EditorGUILayout.ObjectField("NPC Stats: ", Target.Stats, typeof(NPCStats), false) as NPCStats;
            }
        }

    }
}
