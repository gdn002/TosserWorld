using UnityEditor;

using TosserWorld.Modules.Configurations;
using UnityEditor.SceneManagement;

namespace TosserWorld.Entities
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : Editor
    {
        private Entity Target { get { return target as Entity; } }

        private bool ShowModules = true;


        public override void OnInspectorGUI()
        {
            Target.Name = EditorGUILayout.TextField("Name: ", Target.Name);

            EditorGUILayout.Space();
            ShowModules = EditorGUILayout.Foldout(ShowModules, "Modules");
            if (ShowModules)
                OnModulesGUI();
        }

        private void OnModulesGUI()
        {
            EditorGUI.BeginChangeCheck();

            Target.EnableAction = EditorGUILayout.ToggleLeft("Action", Target.EnableAction);
            if (Target.EnableAction) Target.ActionConfig = EditorGUILayout.ObjectField(Target.ActionConfig, typeof(ActionConfig), false) as ActionConfig;

            Target.EnableBrain = EditorGUILayout.ToggleLeft("Brain", Target.EnableBrain);
            if (Target.EnableBrain) Target.BrainConfig = EditorGUILayout.ObjectField(Target.BrainConfig, typeof(BrainConfig), false) as BrainConfig;

            Target.EnableInventory = EditorGUILayout.ToggleLeft("Inventory", Target.EnableInventory);
            if (Target.EnableInventory) Target.InventoryConfig = EditorGUILayout.ObjectField(Target.InventoryConfig, typeof(InventoryConfig), false) as InventoryConfig;

            Target.EnableInteraction = EditorGUILayout.ToggleLeft("Interaction", Target.EnableInteraction);
            if (Target.EnableInteraction) Target.InteractionConfig = EditorGUILayout.ObjectField(Target.InteractionConfig, typeof(InteractionConfig), false) as InteractionConfig;

            Target.EnableMovement = EditorGUILayout.ToggleLeft("Movement", Target.EnableMovement);
            if (Target.EnableMovement) Target.MovementConfig = EditorGUILayout.ObjectField(Target.MovementConfig, typeof(MovementConfig), false) as MovementConfig;

            Target.EnablePhysics = EditorGUILayout.ToggleLeft("Physics", Target.EnablePhysics);
            if (Target.EnablePhysics) Target.PhysicsConfig = EditorGUILayout.ObjectField(Target.PhysicsConfig, typeof(PhysicsConfig), false) as PhysicsConfig;

            Target.EnableStacking = EditorGUILayout.ToggleLeft("Stacking", Target.EnableStacking);
            if (Target.EnableStacking) Target.StackingConfig = EditorGUILayout.ObjectField(Target.StackingConfig, typeof(StackingConfig), false) as StackingConfig;

            Target.EnableStats = EditorGUILayout.ToggleLeft("Stats", Target.EnableStats);
            if (Target.EnableStats) Target.StatsConfig = EditorGUILayout.ObjectField(Target.StatsConfig, typeof(StatsConfig), false) as StatsConfig;

            EditorGUILayout.ToggleLeft("TagList", true);
            Target.TagListConfig = EditorGUILayout.ObjectField(Target.TagListConfig, typeof(TagListConfig), false) as TagListConfig;

            //Target.EnableMODULE = EditorGUILayout.ToggleLeft("MODULE", Target.EnableMODULE);
            //if (Target.EnableMODULE) Target.MODULEConfig = EditorGUILayout.ObjectField(Target.MODULEConfig, typeof(MODULEConfig), false) as MODULEConfig;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Target);
                EditorSceneManager.MarkAllScenesDirty();
            }
        }


    }
}
