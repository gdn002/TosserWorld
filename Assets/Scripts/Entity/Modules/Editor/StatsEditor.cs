using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(Stats))]
    public class StatsEditor : Editor
    {
        private Stats Target { get { return target as Stats; } }

        public override void OnInspectorGUI()
        {
            // TODO
        }
    }
}
