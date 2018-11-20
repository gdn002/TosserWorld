using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Entity.Component
{
    [CustomEditor(typeof(BasicData))]
    public class BasicDataEditor : Editor
    {
        private ReorderableList TagList;

        private BasicData Target { get { return target as BasicData; } }

        public void OnEnable()
        {
            TagList = new ReorderableList(Target.Tags, typeof(EntityTags), true, true, true, true);
            TagList.drawHeaderCallback = OnDrawHeader;
            TagList.drawElementCallback = OnDrawElement;
            TagList.onAddCallback = OnAddElement;
        }

        public override void OnInspectorGUI()
        {
            Target.Name = EditorGUILayout.TextField("Name: ", Target.Name);

            if (TagList != null && Target.Tags != null)
                TagList.DoLayoutList();
        }

        //// ---- TAG LIST ----

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Tags");
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            Rect tagPopupRect = new Rect();
            tagPopupRect.Set(rect.x, rect.y + 2, rect.width, rect.height);
            Target.Tags[index] = (EntityTags)EditorGUI.EnumPopup(tagPopupRect, Target.Tags[index]);
        }

        private void OnAddElement(ReorderableList list)
        {
            EntityTags tag = EntityTags.Any;
            Target.Tags.Add(tag);
        }
    }
}
