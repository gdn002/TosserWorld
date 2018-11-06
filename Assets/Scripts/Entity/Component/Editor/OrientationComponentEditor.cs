using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

using Utility.Enumerations;

namespace Entity.Component.Editor
{
    [CustomEditor(typeof(OrientationComponent))]
    public class OrientationComponentEditor : UnityEditor.Editor
    {
        private OrientationComponent Target { get { return target as OrientationComponent; } }

        public override void OnInspectorGUI()
        {
            Target.LocalOrientation = (Orientation)EditorGUILayout.EnumPopup("Orientation: ", Target.LocalOrientation);

            EditorGUILayout.LabelField("Attached SpriteOrientation components: " + Target.SpriteCount());
        }
    }
}
