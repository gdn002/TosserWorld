using UnityEditor;
using UnityEngine;
using TosserWorld.Modules.ActionScripts;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(ActionConfig))]
    public class ActionConfigEditor : Editor
    {
        private ActionConfig Target { get { return target as ActionConfig; } }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();

            Target.RateOfFire = EditorGUILayout.IntField("Rate of Fire: ", Target.RateOfFire);
            EditorGUILayout.LabelField("Time between shots: " + (Target.TimeBetweenShots * 1000) + "ms");

            EditorGUILayout.Space();

            Target.AutoFire = EditorGUILayout.Toggle("Auto Fire ", Target.AutoFire);
            Target.RunAndGun = EditorGUILayout.Toggle("Run and Gun ", Target.RunAndGun);
            Target.ConsumesItem = EditorGUILayout.Toggle("Consumes Item ", Target.ConsumesItem);

            Target.ActionAnimation = (ActionAnimation)EditorGUILayout.EnumPopup("Animation Type: ", Target.ActionAnimation);
            EditorGUILayout.LabelField("Activation frame: " + (Target.ActivationFrame()));

            EditorGUILayout.Space();

            Target.ActionType = (ActionType)EditorGUILayout.EnumPopup("Action Type: ", Target.ActionType);
            OnActionTypeFields();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Target);
            }
        }

        private void OnActionTypeFields()
        {
            switch(Target.ActionType)
            {
                case ActionType.RunScript:
                    Target.SelectedScript = EditorGUILayout.Popup("Action Script: ", Target.SelectedScript, ActionScriptSelector.AllNames());
                    break;

                case ActionType.SpawnPrefab:
                    Target.SpawnPrefab = EditorGUILayout.ObjectField("Prefab: ", Target.SpawnPrefab, typeof(GameObject), false) as GameObject;
                    break;
            }
        }
    }
}
