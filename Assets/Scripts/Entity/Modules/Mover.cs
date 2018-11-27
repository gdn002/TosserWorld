using System;
using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Mover", menuName = "Modules/Mover")]
    public class Mover : Module
    {
        public Vector2 Movement = Vector2.zero;
        public float SpeedLimit = 1;

        public float FrameLimit { get { return SpeedLimit * Time.fixedDeltaTime; } }

        public bool OverridePhysics = false;

        protected override Module Clone()
        {
            Mover clone = CreateInstance<Mover>();

            clone.Movement = Movement;
            clone.SpeedLimit = SpeedLimit;

            return clone;
        }

        public override void LateUpdate()
        {
            if (!OverridePhysics)
            {
                // Movement should not override physics
                var physics = Owner.GetModule<PhysicsObject>();
                if (physics != null && physics.Active)
                    return;
            }

            Owner.RigidBody.velocity = Movement;
        }

        /// <summary>
        /// Stop the entity's movement.
        /// </summary>
        public void Stop()
        {
            Movement = Vector2.zero;
        }

        /// <summary>
        /// Makes the entity move towards the direction of a vector.
        /// </summary>
        /// <param name="vector">The direction to move towards</param>
        /// <param name="limit">True if the movement should obey the speed limit (default)</param>
        public void Move(Vector2 vector, bool limit = true)
        {
            if (limit)
            {
                // Clamp to walk speed
                vector = Vector2.ClampMagnitude(vector, SpeedLimit);
            }

            Movement = vector;
        }

        /// <summary>
        /// Makes the entity move towards the direction of the vector exactly at the speed limit.
        /// </summary>
        /// <param name="vector">The direction to move towards</param>
        public void MoveFull(Vector2 vector)
        {
            vector = vector.normalized * SpeedLimit;
            Move(vector, false);
        }

        /// <summary>
        /// Makes the entity move towards the direction of a vector in screen space.
        /// </summary>
        /// <param name="vector">The direction to move towards</param>
        /// <param name="limit">True if the movement should obey the speed limit (default)</param>
        public void MoveScreen(Vector2 vector, bool limit = true)
        {
            // Rotate walk vector to match camera rotation
            vector = Quaternion.Euler(0, 0, 45) * vector;
            vector = Quaternion.Euler(0, 0, CameraController.Controller.Orientation.RotationAngle()) * vector;

            // Call Walk() using the rotated vector
            Move(vector, limit);
        }

        /// <summary>
        /// Makes the entity move towards the direction of a vector in screen space exactly at the speed limit.
        /// </summary>
        /// <param name="vector">The direction to move towards</param>
        public void MoveScreenFull(Vector2 vector)
        {
            // Rotate walk vector to match camera rotation
            vector = Quaternion.Euler(0, 0, 45) * vector;
            vector = Quaternion.Euler(0, 0, CameraController.Controller.Orientation.RotationAngle()) * vector;

            // Call Walk() using the rotated vector
            MoveFull(vector);
        }
    }
}
