using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Entity.Component
{
    [CustomEditor(typeof(BasicInventory))]
    public class EntityInventoryEditor : Editor
    {
        private BasicInventory Target { get { return target as BasicInventory; } }

        public override void OnInspectorGUI()
        {
            Target.Rows = EditorGUILayout.IntField("Rows: ", Target.Rows);
            Target.Cols = EditorGUILayout.IntField("Cols: ", Target.Cols);

            EditorGUILayout.LabelField("Slot count: " + Target.SlotCount);
        }
    }
}
