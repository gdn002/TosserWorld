using UnityEditor;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(MovementConfig))]
    public class MovementConfigEditor : Editor
    {
        private MovementConfig Target { get { return target as MovementConfig; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SpeedLimit = EditorGUILayout.Slider("Speed limit: ", Target.SpeedLimit, 0, 10);
            EditorGUILayout.SelectableLabel("Speed limit/frame: " + Target.FrameLimit.ToString());

            EditorUtility.SetDirty(target);
        }
    }
}
