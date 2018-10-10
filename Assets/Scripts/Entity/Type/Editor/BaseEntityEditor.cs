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

        private BaseEntity Entity { get { return target as BaseEntity; } }

        public void OnEnable()
        {
            TagList = new ReorderableList(Entity.Tags, typeof(EntityTags), true, true, true, true);
            TagList.drawHeaderCallback = OnDrawHeader;
            TagList.drawElementCallback = OnDrawElement;
            TagList.onAddCallback = OnAddElement;
        }

        public override void OnInspectorGUI()
        {
            ShowBaseEntity = EditorGUILayout.Foldout(ShowBaseEntity, "Base Entity");
            if (ShowBaseEntity)
            {
                Entity.EntityName = EditorGUILayout.TextField("Entity name: ", Entity.EntityName);
                Entity.DefaultInteraction = (EntityInteractions)EditorGUILayout.EnumPopup("Right click interaction: ", Entity.DefaultInteraction);

                if (TagList != null && Entity.Tags != null)
                    TagList.DoLayoutList();

                Entity.InventorySprite = EditorGUILayout.ObjectField("Inventory sprite: ", Entity.InventorySprite, typeof(Sprite), false) as Sprite;
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
            Entity.Tags[index] = (EntityTags)EditorGUI.EnumPopup(tagPopupRect, Entity.Tags[index]);
        }

        private void OnAddElement(ReorderableList list)
        {
            EntityTags tag = EntityTags.Any;
            Entity.Tags.Add(tag);
        }
    }
}
