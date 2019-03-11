using UnityEditor;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(InventoryConfig))]
    public class InventoryConfigEditor : Editor
    {
        private InventoryConfig Target { get { return target as InventoryConfig; } }

        public override void OnInspectorGUI()
        {
            Target.Rows = EditorGUILayout.IntField("Rows: ", Target.Rows);
            Target.Cols = EditorGUILayout.IntField("Cols: ", Target.Cols);

            EditorGUILayout.LabelField("Slot count: " + Target.SlotCount);

            EditorUtility.SetDirty(target);
        }
    }
}
