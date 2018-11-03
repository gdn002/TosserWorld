using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Entity.Type
{
    [CustomEditor(typeof(NPCEntity))]
    public class NPCEntityEditor : BaseEntityEditor
    {
        private bool ShowNPCEntity = true;

        private NPCEntity Entity { get { return target as NPCEntity; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowNPCEntity = EditorGUILayout.Foldout(ShowNPCEntity, "NPC Entity");
            if (ShowNPCEntity)
            {
                Entity.Speed = EditorGUILayout.Slider("Speed: ", Entity.Speed, 0, 10);
                EditorGUILayout.SelectableLabel("Speed/Frame: " + Entity.FrameSpeed.ToString());
            }
        }

    }
}
