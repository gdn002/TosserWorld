using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;

namespace Entity.Component
{
    public class PhysicsComponent : BaseComponent
    {
        public class PhysicsState
        {
            // Vertical movement data
            public float VerticalVelocity;
            public float Height;

            public bool Active = false;
            public bool Grounded { get { return Height <= 0; } }
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

        // Shortcuts
        private Rigidbody2D RigidBody { get { return Owner.Components.RigidBody; } }
        private Collider2D MainCollider { get { return Owner.Components.MainCollider; } }

        // Physics state data
        public PhysicsState State { get; private set; }

        private float ScaledGravity { get { return Physics2D.gravity.y * GravityScale; } }

        // Use this for initialization
        protected override void OnStart()
        {
            State = new PhysicsState();

            InitializeEntitySpace();
        }

        private void InitializeEntitySpace()
        {
            GameObject obj = new GameObject();
            obj.name = "Space";
            obj.layer = LayerMask.NameToLayer("Entity");

            obj.transform.SetParent(transform, false);

            GenerateEntitySpaceCollider(obj);
        }

        private void GenerateEntitySpaceCollider(GameObject obj)
        {
            // Copy the main collider as this entity's spatial collider
            System.Type type = MainCollider.GetType();
            var spaceCollider = obj.AddComponent(type);

            // NOT SUPPORTED: Composite and Edge colliders
            if (type == typeof(CircleCollider2D))
            {
                CircleCollider2D from = MainCollider as CircleCollider2D;
                CircleCollider2D to = spaceCollider as CircleCollider2D;

                to.offset = from.offset;
                to.radius = from.radius;
            }
            else if (type == typeof(BoxCollider2D))
            {
                BoxCollider2D from = MainCollider as BoxCollider2D;
                BoxCollider2D to = spaceCollider as BoxCollider2D;

                to.offset = from.offset;
                to.size = from.size;
                to.edgeRadius = from.edgeRadius;
            }
            else if (type == typeof(CapsuleCollider2D))
            {
                CapsuleCollider2D from = MainCollider as CapsuleCollider2D;
                CapsuleCollider2D to = spaceCollider as CapsuleCollider2D;

                to.offset = from.offset;
                to.size = from.size;
                to.direction = from.direction;
            }
            else if (type == typeof(PolygonCollider2D))
            {
                PolygonCollider2D from = MainCollider as PolygonCollider2D;
                PolygonCollider2D to = spaceCollider as PolygonCollider2D;

                to.offset = from.offset;
                to.points = from.points;
            }

            (spaceCollider as Collider2D).isTrigger = true;
        }

        /// <summary>
        /// Puts entity in physics mode.
        /// Enables the full range of gravity, bouncing and friction calculations to operate over the entity.
        /// Automatically enabled when moving the object via any PhysicsComponent functions and disabled once the object reaches a complete standstill.
        /// </summary>
        /// <returns></returns>
        public void EnablePhysicsMode()
        {
            State.Active = true;
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

        void FixedUpdate()
        {
            if (VerifyPhysicsMode())
            {
                HandlePhysics();
            }
        }

        private bool VerifyPhysicsMode()
        {
            if (!State.Active)
            {
                return false;
            }

            if (IsCurrentlyStill() && State.Height == 0)
            {
                State.Active = false;
            }

            return State.Active;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.rigidbody != null)
            {
                if (EnableOnCollisions)
                    EnablePhysicsMode();
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            
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

            MainCollider.enabled = State.Grounded;
        }

        private void HandleGravity()
        {
            if (!State.Grounded)
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