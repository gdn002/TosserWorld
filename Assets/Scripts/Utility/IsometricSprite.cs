using UnityEngine;
using UnityEngine.Rendering;

namespace Utility
{
    /// <summary>
    /// Handles several isometric functions for sprite objects
    /// </summary>
    public class IsometricSprite : MonoBehaviour
    {
        public bool Enabled = true;

        private SortingGroup SortGroup;
        private SpriteRenderer Renderer;

        void Start()
        {
            SortGroup = GetComponent<SortingGroup>();
            Renderer = GetComponent<SpriteRenderer>();

            if (SortGroup != null)
            {
                SortGroup.sortingOrder = 1;
            }
            if (Renderer != null)
            {
                Renderer.sortingOrder = 1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Enabled)
            {
                transform.rotation = CameraController.Camera.transform.rotation;
                //IsometricSorting();
            }
        }

        private void IsometricSorting()
        {
            // Sort sprites by rotating their world position by 45 degrees and measuring them along the X axis
            float position = CameraController.Controller.Orientation.RelativeVertical(transform.position);
            int sort = (int)(-position * 1000);

            if (SortGroup != null)
            {
                SortGroup.sortingOrder = sort;
            }
            if (Renderer != null)
            {
                Renderer.sortingOrder = sort;
            }
        }

        public void ResetRotation()
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}
