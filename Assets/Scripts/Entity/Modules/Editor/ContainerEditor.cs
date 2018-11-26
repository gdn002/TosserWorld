using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : Editor
    {
        private Container Target { get { return target as Container; } }

        public override void OnInspectorGUI()
        {
            Target.Rows = EditorGUILayout.IntField("Rows: ", Target.Rows);
            Target.Cols = EditorGUILayout.IntField("Cols: ", Target.Cols);

            EditorGUILayout.LabelField("Slot count: " + Target.SlotCount);
        }
    }
}
