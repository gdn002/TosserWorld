using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Entity.Type
{
    [CustomEditor(typeof(NPCEntity))]
    public class NPCEntityEditor : BaseEntityEditor
    {
        private bool ShowNPCEntity = true;

        private NPCEntity Entity { get { return target as NPCEntity; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowNPCEntity = EditorGUILayout.Foldout(ShowNPCEntity, "NPC Entity");
            if (ShowNPCEntity)
            {
                Entity.WalkSpeed = EditorGUILayout.Slider("Walk speed: ", Entity.WalkSpeed, 0, 100);
                EditorGUILayout.SelectableLabel("Walk delta: " + Entity.WalkDelta.ToString());

                Entity.HandSprite = EditorGUILayout.ObjectField("Hand sprite: ", Entity.HandSprite, typeof(SpriteRenderer), true) as SpriteRenderer;
            }
        }

    }
}
