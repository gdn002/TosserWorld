using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Utility.Enumerations;
using Utility;

namespace Entity.Component
{
    public class OrientationComponent : MonoBehaviour
    {
        public Orientation LocalOrientation = Utility.Enumerations.Orientation.N;

        private Orientation? LastCamera = null;
        private Orientation? LastLocal = null;

        private SpriteOrientation[] Sprites;

        void Start()
        {
            Sprites = GetComponentsInChildren<SpriteOrientation>();
        }

        void Update()
        {
            if (LastCamera == null || LastCamera != CameraController.Controller.Orientation.CurrentOrientation || LastLocal != LocalOrientation)
            {
                LastCamera = CameraController.Controller.Orientation.CurrentOrientation;
                LastLocal = LocalOrientation;

                foreach (var sprite in Sprites)
                {
                    sprite.UpdateOrientation(LocalOrientation, LastCamera.Value);
                }
            }
        }

        public int SpriteCount()
        {
            return GetComponentsInChildren<SpriteOrientation>().Length;
        }
    }
}
