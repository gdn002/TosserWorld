using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Utility.Enumerations;

namespace Utility
{
    [CustomEditor(typeof(SpriteOrientation))]
    public class SpriteOrientationEditor : Editor
    {
        private SpriteOrientation Target { get { return target as SpriteOrientation; } }

        public override void OnInspectorGUI()
        {
            Target.OrientationMode = (SpriteOrientation.Mode)EditorGUILayout.EnumPopup("Orientation mode: ", Target.OrientationMode);

            switch (Target.OrientationMode)
            {
                case SpriteOrientation.Mode.None:
                    OnNone();
                    break;
                case SpriteOrientation.Mode.Quarter:
                    OnQuarter();
                    break;
                case SpriteOrientation.Mode.Half:
                    OnHalf();
                    break;
                case SpriteOrientation.Mode.Full:
                    OnFull();
                    break;
            }
        }

        private void OnNone()
        {
            Target.Sprites[0] = EditorGUILayout.ObjectField("All: ", Target.Sprites[0], typeof(Sprite), false) as Sprite;
        }

        private void OnQuarter()
        {
            Target.Sprites[0] = EditorGUILayout.ObjectField("North/South: ", Target.Sprites[0], typeof(Sprite), false) as Sprite;
            Target.Sprites[2] = EditorGUILayout.ObjectField("East/West: ", Target.Sprites[2], typeof(Sprite), false) as Sprite;
            Target.Sprites[1] = EditorGUILayout.ObjectField("NE/SW and SE/NW: ", Target.Sprites[1], typeof(Sprite), false) as Sprite;
        }

        private void OnHalf()
        {
            Target.Sprites[0] = EditorGUILayout.ObjectField("North: "           , Target.Sprites[0], typeof(Sprite), false) as Sprite;
            Target.Sprites[4] = EditorGUILayout.ObjectField("South: "           , Target.Sprites[4], typeof(Sprite), false) as Sprite;
            Target.Sprites[2] = EditorGUILayout.ObjectField("East and West: "   , Target.Sprites[2], typeof(Sprite), false) as Sprite;
            Target.Sprites[1] = EditorGUILayout.ObjectField("NE and NW: "       , Target.Sprites[1], typeof(Sprite), false) as Sprite;
            Target.Sprites[3] = EditorGUILayout.ObjectField("SE and SW: "       , Target.Sprites[3], typeof(Sprite), false) as Sprite;
        }

        private void OnFull()
        {
            Target.Sprites[0] = EditorGUILayout.ObjectField("North: "       , Target.Sprites[0], typeof(Sprite), false) as Sprite;
            Target.Sprites[1] = EditorGUILayout.ObjectField("Northeast: "   , Target.Sprites[1], typeof(Sprite), false) as Sprite;
            Target.Sprites[2] = EditorGUILayout.ObjectField("East: "        , Target.Sprites[2], typeof(Sprite), false) as Sprite;
            Target.Sprites[3] = EditorGUILayout.ObjectField("Southeast: "   , Target.Sprites[3], typeof(Sprite), false) as Sprite;
            Target.Sprites[4] = EditorGUILayout.ObjectField("South: "       , Target.Sprites[4], typeof(Sprite), false) as Sprite;
            Target.Sprites[5] = EditorGUILayout.ObjectField("Southwest: "   , Target.Sprites[5], typeof(Sprite), false) as Sprite;
            Target.Sprites[6] = EditorGUILayout.ObjectField("West: "        , Target.Sprites[6], typeof(Sprite), false) as Sprite;
            Target.Sprites[7] = EditorGUILayout.ObjectField("Northwest: "   , Target.Sprites[7], typeof(Sprite), false) as Sprite;
        }
    }
}
