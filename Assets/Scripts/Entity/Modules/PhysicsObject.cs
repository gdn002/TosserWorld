using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Physics Object", menuName = "Modules/Physics Object")]
    public class PhysicsObject : Module
    {
        // ---- Read-only properties ----

        public float VerticalVelocity { get; private set; }
        public float Height { get; private set; }

        public bool Active { get; private set; }
        public bool Grounded { get { return Height <= 0; } }

        private float ScaledGravity { get { return Physics2D.gravity.y * GravityScale; } }

        // ----

        // The child transform contains the actual item which will move vertically
        public Transform PhysicsBody;

        // ---- Configurable properties ----

        // Drag value for vertical movement
        public float AirDrag;

        // The object's gravity scale
        public float GravityScale;

        // The object's bounciness
        public float Bounciness;

        // The object's friction modifier
        public float Friction;

        public bool EnableOnCollisions;

        // ---- Shortcuts ----

        private Rigidbody2D RigidBody { get { return Owner.RigidBody; } }
        private Collider2D MainCollider { get { return Owner.MainCollider; } }


        public PhysicsObject()
        {
            AirDrag = 1;
            GravityScale = 1;
            Bounciness = 0.1f;
            Friction = 1;

            EnableOnCollisions = true;
        }

        protected override Module Clone()
        {
            PhysicsObject clone = CreateInstance<PhysicsObject>();

            clone.AirDrag = AirDrag;
            clone.GravityScale = GravityScale;
            clone.Bounciness = Bounciness;
            clone.Friction = Friction;

            clone.EnableOnCollisions = EnableOnCollisions;

            return clone;
        }

        // Use this for initialization
        protected override void OnInitialize()
        {
            Active = false;

            // TODO: This is just a temporary hack
            PhysicsBody = Owner.transform.Find("Render").Find("Body");

            InitializeEntitySpace();
        }

        private void InitializeEntitySpace()
        {
            GameObject obj = new GameObject();
            obj.name = "Space";
            obj.layer = LayerMask.NameToLayer("Entity");
            obj.transform.SetParent(Owner.transform, false);

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
            Active = true;
        }

        /// <summary>
        /// Returns true when the object is completely at a standstill.
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentlyStill()
        {
            return RigidBody.velocity.magnitude == 0 && VerticalVelocity == 0;
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
            VerticalVelocity += acc.y;

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

            Height = height;

            EnablePhysicsMode();
        }

        /// <summary>
        /// Brings the object to a complete standstill.
        /// </summary>
        public void ResetVelocity()
        {
            RigidBody.velocity = Vector2.zero;
            VerticalVelocity = 0;
        }

        private bool ShouldSimulate()
        {
            if (!Active)
            {
                return false;
            }

            if (IsCurrentlyStill() && Height == 0)
            {
                Active = false;
            }

            return Active;
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

        public override void LateUpdate()
        {
            SimulatePhysics();
        }

        /// <summary>
        /// Run a FixedUpdate's worth of physics simulation
        /// </summary>
        private void SimulatePhysics()
        {
            if (!ShouldSimulate())
                return;

            SimulateGravity();
            SimulateDrag();
            SimulateBounce();
            SimulateFriction();

            // Object changes height according to its vertical velocity
            Height += VerticalVelocity * Time.fixedDeltaTime;

            // Move child object to correct position
            PhysicsBody.localPosition = new Vector3(0, Height, 0);

            MainCollider.enabled = Grounded;
        }

        private void SimulateGravity()
        {
            if (!Grounded)
            {
                // If the height is over zero, the object is in the air and should have gravity force applied to it
                VerticalVelocity += ScaledGravity * Time.fixedDeltaTime;
            }
        }

        private void SimulateDrag()
        {
            // Drag is only applied if moving
            if (VerticalVelocity != 0)
            {
                // Drag FORCE formula
                float dragForce = 0.5f * Mathf.Pow(VerticalVelocity, 2) * AirDrag;

                // Drag ACCELERATION formula
                float dragAcc = dragForce / RigidBody.mass;

                // Drag is always applied against the direction of the velocity
                if (VerticalVelocity > 0)
                {
                    VerticalVelocity -= dragAcc * Time.fixedDeltaTime;
                }
                else
                {
                    VerticalVelocity += dragAcc * Time.fixedDeltaTime;
                }
            }
        }

        private void SimulateBounce()
        {
            // If the height is under zero, that means the object has hit the ground
            if (Height < 0 && VerticalVelocity < 0)
            {
                // Redirect the current vertical velocity to create bounce
                VerticalVelocity *= -Bounciness;

                // Also trim some of the ground velocity
                RigidBody.velocity = RigidBody.velocity * Bounciness;

                // Cutoff point for bouncing
                if (VerticalVelocity < 4f)
                {
                    VerticalVelocity = 0;
                    Height = 0;
                }
            }
        }

        private void SimulateFriction()
        {
            // Friction only applies if the object is touching the ground (Height == 0)
            if (Height == 0)
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