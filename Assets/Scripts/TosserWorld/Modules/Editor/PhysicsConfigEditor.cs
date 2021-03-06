﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TosserWorld.Modules.Configurations
{
    [CustomEditor(typeof(PhysicsConfig))]
    public class PhysicsConfigEditor : Editor
    {
        private bool ShowEnable = true;

        private PhysicsConfig Target { get { return target as PhysicsConfig; } }

        public override void OnInspectorGUI()
        {
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
