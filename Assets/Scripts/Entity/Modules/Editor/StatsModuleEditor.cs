using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(StatsModule))]
    public class StatsModuleEditor : Editor
    {
        private StatsModule Target { get { return target as StatsModule; } }

        public override void OnInspectorGUI()
        {
            // TODO

            EditorUtility.SetDirty(target);
        }
    }
}
