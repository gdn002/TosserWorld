using System.Collections;
using UnityEngine;
using UnityEditor;

using Entity.Component;

namespace Entity.Type
{
    [CustomEditor(typeof(BasicEntity))]
    public class BasicEntityEditor : Editor
    {
        private bool ShowBasic = true;

        private BasicEntity Target { get { return target as BasicEntity; } }

        public override void OnInspectorGUI()
        {
            ShowBasic = EditorGUILayout.Foldout(ShowBasic, "Basic");
            if (ShowBasic)
            {
                Target.InventorySprite = EditorGUILayout.ObjectField("Inventory sprite: ", Target.InventorySprite, typeof(Sprite), false) as Sprite;

                Target.Data         = EditorGUILayout.ObjectField("Data: "      , Target.Data, typeof(BasicData), false)           as BasicData;
                Target.Inventory    = EditorGUILayout.ObjectField("Inventory: " , Target.Inventory, typeof(BasicInventory), false) as BasicInventory;
                Target.Physics      = EditorGUILayout.ObjectField("Physics: "   , Target.Physics, typeof(BasicPhysics), false)     as BasicPhysics;
                Target.Stack        = EditorGUILayout.ObjectField("Stack: "     , Target.Stack, typeof(BasicStack), false)         as BasicStack;
            }
        }

        
    }
}
