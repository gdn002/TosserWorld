using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Entity.Component
{
    [CustomEditor(typeof(PhysicsComponent))]
    public class PhysicsComponentEditor : Editor
    {
        private bool ShowEnable = true;

        private PhysicsComponent Entity { get { return target as PhysicsComponent; } }

        public override void OnInspectorGUI()
        {
            Entity.PhysicsBody = EditorGUILayout.ObjectField("Physics body: ", Entity.PhysicsBody, typeof(Transform), true) as Transform;

            Entity.GravityScale = EditorGUILayout.FloatField("Gravity scale: ", Entity.GravityScale);
            Entity.Friction = EditorGUILayout.Slider("Ground friction: ", Entity.Friction, 0, 2);
            Entity.AirDrag = EditorGUILayout.Slider("Air drag: ", Entity.AirDrag, 0, 2);
            Entity.Bounciness = EditorGUILayout.Slider("Bounciness: ", Entity.Bounciness, 0, 2);

            ShowEnable = EditorGUILayout.Foldout(ShowEnable, "Enable Physics On");
            if (ShowEnable)
            {
                Entity.EnableOnCollisions = EditorGUILayout.Toggle("Collisions", Entity.EnableOnCollisions);
            }
        }
    }
}
