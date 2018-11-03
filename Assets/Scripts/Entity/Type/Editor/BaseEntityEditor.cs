using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Entity.Type
{
    [CustomEditor(typeof(BaseEntity))]
    public class BaseEntityEditor : Editor
    {
        private bool ShowBaseEntity = true;
        private ReorderableList TagList;

        private BaseEntity Target { get { return target as BaseEntity; } }

        public void OnEnable()
        {
            TagList = new ReorderableList(Target.Tags, typeof(EntityTags), true, true, true, true);
            TagList.drawHeaderCallback = OnDrawHeader;
            TagList.drawElementCallback = OnDrawElement;
            TagList.onAddCallback = OnAddElement;
        }

        public override void OnInspectorGUI()
        {
            ShowBaseEntity = EditorGUILayout.Foldout(ShowBaseEntity, "Base Entity");
            if (ShowBaseEntity)
            {
                Target.Name = EditorGUILayout.TextField("Entity name: ", Target.Name);
                Target.DefaultInteraction = (EntityInteractions)EditorGUILayout.EnumPopup("Right click interaction: ", Target.DefaultInteraction);

                if (TagList != null && Target.Tags != null)
                    TagList.DoLayoutList();

                Target.InventorySprite = EditorGUILayout.ObjectField("Inventory sprite: ", Target.InventorySprite, typeof(Sprite), false) as Sprite;
            }
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
