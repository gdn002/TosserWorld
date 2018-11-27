using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(Mover))]
    public class MoverEditor : Editor
    {
        private Mover Target { get { return target as Mover; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SpeedLimit = EditorGUILayout.Slider("Speed limit: ", Target.SpeedLimit, 0, 10);
            EditorGUILayout.SelectableLabel("Speed limit/frame: " + Target.FrameLimit.ToString());
        }
    }
}
