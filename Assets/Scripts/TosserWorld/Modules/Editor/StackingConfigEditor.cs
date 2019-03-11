using UnityEditor;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(StackingConfig))]
    public class StackingConfigEditor : Editor
    {
        private StackingConfig Target { get { return target as StackingConfig; } }

        public override void OnInspectorGUI()
        {
            Target.MaxAmount = EditorGUILayout.IntField("Max Amount: ", Target.MaxAmount);

            EditorGUILayout.LabelField("Is stackable: " + Target.IsStackable);

            EditorUtility.SetDirty(target);
        }
    }
}
