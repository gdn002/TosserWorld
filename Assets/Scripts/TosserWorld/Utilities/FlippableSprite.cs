using System.Collections;
using UnityEngine;

namespace TosserWorld.Utilities
{
    // Utility for flipping sprites around
    public class FlippableSprite : MonoBehaviour
    {
        public float FlipDuration = 0.1f;

        // true == right; false == left
        private bool Direction = true;

        /// <summary>
        /// Flips the object left or right according to a direction in world space
        /// </summary>
        /// <param name="direction">The direction in world space</param>
        public void FlipTo(Vector2 direction)
        {
            // Rotate vector to match camera rotation
            direction = Quaternion.Euler(0, 0, -45) * direction;
            direction = Quaternion.Euler(0, 0, -CameraController.Controller.Orientation.RotationAngle()) * direction;

            FlipToScreen(direction);
        }

        /// <summary>
        /// Flips the object left or right according to a direction in screen space
        /// </summary>
        /// <param name="direction">The direction in screen space</param>
        public void FlipToScreen(Vector2 direction)
        {
            if (direction.x > 0)
            {
                FlipTo(true);
            }
            else if (direction.x < 0)
            {
                FlipTo(false);
            }
        }

        /// <summary>
        /// Flips the object left or right
        /// </summary>
        /// <param name="direction">The direction: true for right, false for left</param>
        public void FlipTo(bool direction)
        {
            StartCoroutine(Flip(direction));
        }

        private IEnumerator Flip(bool direction)
        {
            if (direction != Direction)
            {
                float to = direction ? 1 : -1;
                float from = -to;

                float current = from;
                float elapsed = 0;

                Direction = direction;

                while (elapsed < FlipDuration)
                {
                    elapsed += Time.deltaTime;
                    current = Mathf.Lerp(from, to, elapsed / FlipDuration);

                    transform.localScale = new Vector3(current, 1, 1);

                    yield return null;
                }

                transform.localScale = new Vector3(to, 1, 1);
            }
        }
    }
}
