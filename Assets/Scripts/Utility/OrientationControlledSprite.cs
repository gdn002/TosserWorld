using UnityEngine;
using Utility.Enumerations;

namespace Utility
{
    public class OrientationControlledSprite : MonoBehaviour
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
                case Orientation.N:
                case Orientation.S:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case Orientation.E:
                case Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case Orientation.NE:
                case Orientation.SW:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case Orientation.SE:
                case Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
            }
        }

        private void Half(Orientation local, Orientation camera)
        {
            switch(RelativeOrientation(local, camera))
            {
                case Orientation.N:
                    Renderer.sprite = Sprites[0];
                    Flip(false);
                    break;
                case Orientation.S:
                    Renderer.sprite = Sprites[4];
                    Flip(false);
                    break;
                case Orientation.E:
                    Renderer.sprite = Sprites[2];
                    Flip(false);
                    break;
                case Orientation.W:
                    Renderer.sprite = Sprites[2];
                    Flip(true);
                    break;
                case Orientation.NE:
                    Renderer.sprite = Sprites[1];
                    Flip(false);
                    break;
                case Orientation.NW:
                    Renderer.sprite = Sprites[1];
                    Flip(true);
                    break;
                case Orientation.SE:
                    Renderer.sprite = Sprites[3];
                    Flip(false);
                    break;
                case Orientation.SW:
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
            if (value < (int)Orientation.N)
            {
                value += (int)Orientation.NW + 1;
            }

            return (Orientation)value;
        }

        private void Flip(bool flip)
        {
            transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        }
    }
}
