using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(MovementModule))]
    public class MovementModuleEditor : Editor
    {
        private MovementModule Target { get { return target as MovementModule; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SpeedLimit = EditorGUILayout.Slider("Speed limit: ", Target.SpeedLimit, 0, 10);
            EditorGUILayout.SelectableLabel("Speed limit/frame: " + Target.FrameLimit.ToString());

            EditorUtility.SetDirty(target);
        }
    }
}
