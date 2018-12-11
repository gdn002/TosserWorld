using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(ContainerModule))]
    public class ContainerModuleEditor : Editor
    {
        private ContainerModule Target { get { return target as ContainerModule; } }

        public override void OnInspectorGUI()
        {
            Target.Rows = EditorGUILayout.IntField("Rows: ", Target.Rows);
            Target.Cols = EditorGUILayout.IntField("Cols: ", Target.Cols);

            EditorGUILayout.LabelField("Slot count: " + Target.SlotCount);

            EditorUtility.SetDirty(target);
        }
    }
}
