using System.Collections;
using UnityEngine;
using UnityEditor;

using TosserWorld.Modules;
using UnityEditorInternal;

namespace TosserWorld.Type
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : Editor
    {
        private ReorderableList ModuleList;

        private Entity Target { get { return target as Entity; } }

        public void OnEnable()
        {
            ModuleList = new ReorderableList(Target.Modules, typeof(Module), true, true, true, true);
            ModuleList.drawHeaderCallback = OnDrawHeader;
            ModuleList.drawElementCallback = OnDrawElement;
            ModuleList.onAddCallback = OnAddElement;
        }

        public override void OnInspectorGUI()
        {
            if (ModuleList != null && Target.Modules != null)
                ModuleList.DoLayoutList();
        }

        // ---- MODULE LIST ----

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Modules");
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            Rect tagPopupRect = new Rect();
            tagPopupRect.Set(rect.x, rect.y, rect.width, rect.height);
            Target.Modules[index] = EditorGUI.ObjectField(tagPopupRect, Target.Modules[index], typeof(Module), false) as Module;
        }

        private void OnAddElement(ReorderableList list)
        {
            Target.Modules.Add(null);
        }
    }
}
