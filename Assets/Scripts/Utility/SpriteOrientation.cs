using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utility
{
    public class SpriteOrientation : MonoBehaviour
    {
        public enum Mode
        {
            None = 0,
            Quarter,
            Half,
            Full,
        }

        public Mode OrientationMode = Mode.None;
        public CameraController.CameraOrientation.Orientation Orientation = CameraController.CameraOrientation.Orientation.N;

        public Sprite[] Sprites = new Sprite[8];

        private SpriteRenderer Renderer;
        private CameraController.CameraOrientation.Orientation? Current = null;
        

        void Start()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            UpdateOrientation();
        }

        private void UpdateOrientation()
        {
            if (Current == null || Current != CameraController.Controller.Orientation.CurrentOrientation)
            {
                Current = CameraController.Controller.Orientation.CurrentOrientation;
                switch (OrientationMode)
                {
                    case Mode.None:
                        None();
                        break;
                    case Mode.Quarter:
                        Quarter();
                        break;
                    case Mode.Half:
                        Half();
                        break;
                    case Mode.Full:
                        Half();
                        break;
                }
            }
        }

        private void None()
        {
            Renderer.sprite = Sprites[0];
            Flip(false);
        }

        private void Quarter()
        {
            switch (RelativeOrientation())
            {
                case CameraController.CameraOrientation.Orientation.N:
                case CameraController.CameraOrientation.Orientation.S:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.E:
                case CameraController.CameraOrientation.Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.NE:
                case CameraController.CameraOrientation.Orientation.SW:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.SE:
                case CameraController.CameraOrientation.Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
            }
        }

        private void Half()
        {
            switch(RelativeOrientation())
            {
                case CameraController.CameraOrientation.Orientation.N:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.S:
                    Renderer.sprite = Sprites[4];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.E:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(true);
                    break;
                case CameraController.CameraOrientation.Orientation.NE:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
                case CameraController.CameraOrientation.Orientation.SE:
                    Renderer.sprite = Sprites[3];
                    Flip(false);
                    break;
                case CameraController.CameraOrientation.Orientation.SW:
                    Renderer.sprite = Sprites[3];
                    Flip(true);
                    break;
            }
        }

        private void Full()
        {
            Renderer.sprite = Sprites[(int)RelativeOrientation()];
            Flip(false);
        }

        private CameraController.CameraOrientation.Orientation RelativeOrientation()
        {
            int value = (int)Orientation - (int)Current;
            if (value < 0)
            {
                value += 8;
            }

            return (CameraController.CameraOrientation.Orientation)value;
        }

        private void Flip(bool flip)
        {
            transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        }
    }
}
