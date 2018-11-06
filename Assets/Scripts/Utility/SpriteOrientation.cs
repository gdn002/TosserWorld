using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Utility.Enumerations;

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

        public Sprite[] Sprites = new Sprite[8];

        private SpriteRenderer Renderer;


        void Start()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateOrientation(Orientation local, Orientation camera)
        {
            switch (OrientationMode)
            {
                case Mode.None:
                    None(local, camera);
                    break;
                case Mode.Quarter:
                    Quarter(local, camera);
                    break;
                case Mode.Half:
                    Half(local, camera);
                    break;
                case Mode.Full:
                    Half(local, camera);
                    break;
            }
        }

        private void None(Orientation local, Orientation camera)
        {
            Renderer.sprite = Sprites[0];
            Flip(false);
        }

        private void Quarter(Orientation local, Orientation camera)
        {
            switch (RelativeOrientation(local, camera))
            {
                case Utility.Enumerations.Orientation.N:
                case Utility.Enumerations.Orientation.S:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.E:
                case Utility.Enumerations.Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.NE:
                case Utility.Enumerations.Orientation.SW:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.SE:
                case Utility.Enumerations.Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
            }
        }

        private void Half(Orientation local, Orientation camera)
        {
            switch(RelativeOrientation(local, camera))
            {
                case Utility.Enumerations.Orientation.N:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.S:
                    Renderer.sprite = Sprites[4];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.E:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(true);
                    break;
                case Utility.Enumerations.Orientation.NE:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
                case Utility.Enumerations.Orientation.SE:
                    Renderer.sprite = Sprites[3];
                    Flip(false);
                    break;
                case Utility.Enumerations.Orientation.SW:
                    Renderer.sprite = Sprites[3];
                    Flip(true);
                    break;
            }
        }

        private void Full(Orientation local, Orientation camera)
        {
            Renderer.sprite = Sprites[(int)RelativeOrientation(local, camera)];
            Flip(false);
        }

        private Orientation RelativeOrientation(Orientation local, Orientation camera)
        {
            int value = (int)local - (int)camera;
            if (value < (int)Utility.Enumerations.Orientation.N)
            {
                value += (int)Utility.Enumerations.Orientation.NW + 1;
            }

            return (Orientation)value;
        }

        private void Flip(bool flip)
        {
            transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        }
    }
}
