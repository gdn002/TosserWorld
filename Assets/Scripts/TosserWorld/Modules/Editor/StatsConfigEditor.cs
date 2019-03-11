using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(StatsConfig))]
    public class StatsConfigEditor : Editor
    {
        private StatsConfig Target { get { return target as StatsConfig; } }


        public override void OnInspectorGUI()
        {
            Target.HasHealth = EditorGUILayout.ToggleLeft("Health", Target.HasHealth);
            if (Target.HasHealth) Target.MaxHealth = EditorGUILayout.IntField(Target.MaxHealth);

            Target.HasStamina = EditorGUILayout.ToggleLeft("Stamina", Target.HasStamina);
            if (Target.HasStamina) Target.MaxStamina = EditorGUILayout.IntField(Target.MaxStamina);

            //Target.HasSTAT = EditorGUILayout.ToggleLeft("STAT", Target.HasSTAT);
            //if (Target.HasSTAT) Target.MaxSTAT = EditorGUILayout.IntField(Target.MaxSTAT);
        }
    }
}
