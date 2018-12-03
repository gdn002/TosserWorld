﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TosserWorld.Modules
{
    [CustomEditor(typeof(PhysicsObject))]
    public class PhysicsEditor : Editor
    {
        private bool ShowEnable = true;

        private PhysicsObject Target { get { return target as PhysicsObject; } }

        public override void OnInspectorGUI()
        {
            Target.PhysicsBody = EditorGUILayout.ObjectField("Physics body: ", Target.PhysicsBody, typeof(Transform), true) as Transform;

            Target.GravityScale = EditorGUILayout.FloatField("Gravity scale: ", Target.GravityScale);
            Target.Friction = EditorGUILayout.Slider("Ground friction: ", Target.Friction, 0, 2);
            Target.AirDrag = EditorGUILayout.Slider("Air drag: ", Target.AirDrag, 0, 2);
            Target.Bounciness = EditorGUILayout.Slider("Bounciness: ", Target.Bounciness, 0, 2);

            ShowEnable = EditorGUILayout.Foldout(ShowEnable, "Enable Physics On");
            if (ShowEnable)
            {
                Target.EnableOnCollisions = EditorGUILayout.Toggle("Collisions", Target.EnableOnCollisions);
            }

            EditorUtility.SetDirty(target);
        }
    }
}
