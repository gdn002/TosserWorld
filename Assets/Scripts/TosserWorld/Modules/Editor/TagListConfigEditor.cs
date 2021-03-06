﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(TagListConfig))]
    public class TagListConfigEditor : Editor
    {
        private ReorderableList TagList;

        private TagListConfig Target { get { return target as TagListConfig; } }

        public void OnEnable()
        {
            TagList = new ReorderableList(Target.Tags, typeof(EntityTags), true, true, true, true);
            TagList.drawHeaderCallback = OnDrawHeader;
            TagList.drawElementCallback = OnDrawElement;
            TagList.onAddCallback = OnAddElement;
        }

        public override void OnInspectorGUI()
        {
            if (TagList != null && Target.Tags != null)
                TagList.DoLayoutList();

            EditorUtility.SetDirty(target);
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
