using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;

namespace Entity.Component
{
    public class PhysicsComponent : MonoBehaviour
    {

        class PhysicsState
        {
            // Vertical movement data
            public float VerticalVelocity;
            public float Height;

            public bool IsInPhysicsMode = false;
            public int  CollisionCount  = 0;
        }

        // The child transform contains the actual item which will move vertically
        public Transform PhysicsBody;

        // Drag value for vertical movement
        public float AirDrag = 1;

        // The object's gravity scale
        public float GravityScale = 1;

        // The object's bounciness
        public float Bounciness = 0.1f;

        // The object's friction modifier
        public float Friction = 1;

        public bool EnableOnCollisions = true;

        // Component references
        private Rigidbody2D RigidBody;

        // Physics state data
        private PhysicsState State;

        public bool IsInPhysicsMode { get { return State.IsInPhysicsMode; } }
        public bool IsColliding     { get { return State.CollisionCount > 0; } }

        private float ScaledGravity { get { return Physics2D.gravity.y * GravityScale; } }

        // Use this for initialization
        void Start()
        {
            BaseEntity entity = GetComponent<BaseEntity>();
            if (entity == null)
            {
                Debug.LogWarning("PhysicsComponent in non-entity object: " + name);
            }

            State = new PhysicsState();

            RigidBody = entity.RigidBody;
            if (RigidBody == null)
            {
                Debug.LogWarning("PhysicsComponent could not find a Rigidbody2D in " + transform.name);
            }
        }

        /// <summary>
        /// Puts entity in physics mode.
        /// Enables the full range of gravity, bouncing and friction calculations to operate over the entity.
        /// Automatically enabled when moving the object via any PhysicsComponent functions and disabled once the object reaches a complete standstill.
        /// </summary>
        /// <returns></returns>
        public void EnablePhysicsMode()
        {
            State.IsInPhysicsMode = true;
        }

        /// <summary>
        /// Returns true when the object is completely at a standstill.
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentlyStill()
        {
            return RigidBody.velocity.magnitude == 0 && State.VerticalVelocity == 0;
        }

        /// <summary>
        /// Applies instant force to the entity.
        /// Puts entity in physics mode.
        /// </summary>
        /// <param name="force">Force vector</param>
        public void ApplyForce(Vector3 force)
        {
            // Acceleration formula (F = m * a)
            Vector3 acc = force / RigidBody.mass;

            // Apply acceleration relative to the ground
            // (do note that the ground velocity's Y axis uses the acceleration's Z axis, since its Y axis gets translated to the vertical velocity instead)
            RigidBody.velocity = new Vector2(RigidBody.velocity.x + acc.x, RigidBody.velocity.y + acc.z);

            // Apply acceleration to vertical velocity
            State.VerticalVelocity += acc.y;

            EnablePhysicsMode();
        }

        /// <summary>
        /// Applies force to the entity over a period of time.
        /// Puts entity in physics mode.
        /// </summary>
        /// <param name="force">Force vector</param>
        /// <param name="time">Time in seconds</param>
        public void ApplyForce(Vector3 force, float time)
        {
            // TODO

            EnablePhysicsMode();
        }

        /// <summary>
        /// Set this entity's height. Does not affect current velocity.
        /// Puts entity in physics mode.
        /// </summary>
        /// <param name="height"></param>
        public void SetHeight(float height)
        {
            if (height < 0)
            {
                height = 0;
            }

            State.Height = height;

            EnablePhysicsMode();
        }

        /// <summary>
        /// Brings the object to a complete standstill.
        /// </summary>
        public void ResetVelocity()
        {
            RigidBody.velocity = Vector2.zero;
            State.VerticalVelocity = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            if (VerifyPhysicsMode())
            {
                HandlePhysics();
            }
        }

        private bool VerifyPhysicsMode()
        {
            if (!IsInPhysicsMode)
            {
                return false;
            }

            if (IsCurrentlyStill() && State.Height == 0)
            {
                State.IsInPhysicsMode = false;
            }

            return IsInPhysicsMode;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.rigidbody != null)
            {
                State.CollisionCount++;

                if (EnableOnCollisions)
                    EnablePhysicsMode();
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.rigidbody != null)
            {
                State.CollisionCount--;
            }
        }

        private void HandlePhysics()
        {
            HandleGravity();
            HandleDrag();
            HandleBounce();
            HandleFriction();

            // Object changes height according to its vertical velocity
            State.Height += State.VerticalVelocity * Time.fixedDeltaTime;

            // Move child object to correct position
            PhysicsBody.localPosition = new Vector3(0, State.Height, 0);
        }

        private void HandleGravity()
        {
            if (State.Height > 0)
            {
                // If the height is over zero, the object is in the air and should have gravity force applied to it
                State.VerticalVelocity += ScaledGravity * Time.fixedDeltaTime;
            }
        }

        private void HandleDrag()
        {
            // Drag is only applied if moving
            if (State.VerticalVelocity != 0)
            {
                // Drag FORCE formula
                float dragForce = 0.5f * Mathf.Pow(State.VerticalVelocity, 2) * AirDrag;

                // Drag ACCELERATION formula
                float dragAcc = dragForce / RigidBody.mass;

                // Drag is always applied against the direction of the velocity
                if (State.VerticalVelocity > 0)
                {
                    State.VerticalVelocity -= dragAcc * Time.fixedDeltaTime;
                }
                else
                {
                    State.VerticalVelocity += dragAcc * Time.fixedDeltaTime;
                }
            }
        }

        private void HandleBounce()
        {
            // If the height is under zero, that means the object has hit the ground
            if (State.Height < 0 && State.VerticalVelocity < 0)
            {
                // Redirect the current vertical velocity to create bounce
                State.VerticalVelocity *= -Bounciness;

                // Also trim some of the ground velocity
                RigidBody.velocity = RigidBody.velocity * Bounciness;

                // Cutoff point for bouncing
                if (State.VerticalVelocity < 4f)
                {
                    State.VerticalVelocity = 0;
                    State.Height = 0;
                }
            }
        }

        private void HandleFriction()
        {
            // Friction only applies if the object is touching the ground (Height == 0)
            if (State.Height == 0)
            {
                // Calculate the max friction force
                float frictionForce = Friction * RigidBody.mass * -ScaledGravity;
                float frictionAcc = frictionForce / RigidBody.mass;

                // The friction vector is applied the exact opposite direction from the object's ground velocity
                Vector2 frictionVector = RigidBody.velocity;
                frictionVector = frictionVector.normalized * (frictionAcc * Time.fixedDeltaTime);

                if (RigidBody.velocity.magnitude < frictionVector.magnitude)
                {
                    // If the friction vector is greater than the current speed, the object stops
                    RigidBody.velocity = Vector2.zero;
                }
                else
                {
                    // Otherwise it simply decelerates
                    RigidBody.velocity = RigidBody.velocity - frictionVector;
                }
            }
        }
    }
}