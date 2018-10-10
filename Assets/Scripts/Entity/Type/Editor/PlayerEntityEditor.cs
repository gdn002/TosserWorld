using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Entity.Type
{
    [CustomEditor(typeof(PlayerEntity))]
    public class PlayerEntityEditor : NPCEntityEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
