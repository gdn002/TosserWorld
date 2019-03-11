using Utility;
using Utility.Enumerations;
using TosserWorld.Entities;

namespace TosserWorld.Utilities
{
    // Controller for keeping orientation data and managing orientation-controlled sprites
    public class EntityOrientation
    {
        public Orientation LocalOrientation = Orientation.N;

        private Orientation? LastCamera = null;
        private Orientation? LastLocal = null;

        private OrientationControlledSprite[] Sprites;

        public void Load(Entity owner)
        {
            Sprites = owner.GetComponentsInChildren<OrientationControlledSprite>();
        }

        public void UpdateSprites()
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
            return Sprites.Length;
        }
    }
}
