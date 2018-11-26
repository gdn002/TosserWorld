using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(Moveable))]
    public class MoveableEditor : Editor
    {
        private Moveable Target { get { return target as Moveable; } }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.SpeedLimit = EditorGUILayout.Slider("Speed limit: ", Target.SpeedLimit, 0, 10);
            EditorGUILayout.SelectableLabel("Speed limit/frame: " + Target.FrameLimit.ToString());
        }
    }
}
