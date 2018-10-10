using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Component;
using Entity.Component.Brain;

namespace Entity.Type
{
    /// <summary>
    /// ENTITY CLASS FOR ALL NPCS
    /// Handles NPC movement and interactions
    /// BaseEntity -> NPCEntity
    /// </summary>
    public class NPCEntity : BaseEntity
    {
        /// <summary>
        /// NPC state information
        /// </summary>
        protected class NPCState
        {
            public NPCState()
            {
                Hitting = false;
                Knockback = false;
            }

            // States
            public bool Hitting { get; set; }             // Is the NPC doing the hit motion?
            public bool Knockback { get; set; }         // Is the NPC currently in knockback?

            /// <summary>
            /// Can the NPC currently move via input?
            /// </summary>
            public bool IsMovementAllowed()
            {
                return !(Hitting || Knockback);
            }
        }

        protected class NPCMovement
        {
            public NPCMovement()
            {
                Direction = true;
            }

            public bool Direction { get; set; }         // true = right, false = left
            public Vector2 WalkVector { get; set; }     // Walking velocity
        }

        //// ---- EDITOR PARAMETERS ----

        // Movement speed modifier
        public float WalkSpeed = 1f;

        // Flip motion duration (in seconds)
        public float FlipDuration = 0.1f;

        //// ---- ADDITIONAL NPC INFORMATION ----

        // Max distance walked per frame
        public float WalkDelta { get { return WalkSpeed * Time.fixedDeltaTime; } }

        //// ---- OTHER ----

        // NPC state
        protected NPCMovement Movement { get; set; }
        protected NPCState State { get; set; }

        // References for entity components
        public InventoryComponent Inventory { get; private set; }
        public BrainComponent Brain { get; private set; }

        // Sub components
        public SpriteRenderer HandSprite;

        public NPCEntity()
        {
            Movement = new NPCMovement();
            State = new NPCState();
        }

        // Initialization
        protected override void OnStart()
        {
            // Detect and fill Entity component references
            Inventory  = GetComponent<InventoryComponent>();
            Brain      = GetComponent<BrainComponent>();

            UpdateHand();
        }

        //// ---- NPC CONTROL FUNCTIONS ----

        /// <summary>
        /// Stop the NPC's movement.
        /// </summary>
        public void Stop()
        {
            Movement.WalkVector = Vector2.zero;
        }

        /// <summary>
        /// Makes the NPC walk towards the direction of the walkVector.
        /// Ignores the NPC's walk speed.
        /// </summary>
        /// <param name="walkVector">The direction to walk towards</param>
        public void WalkRaw(Vector2 walkVector)
        {
            // Set the NPC in motion
            Movement.WalkVector = walkVector;
        }

        /// <summary>
        /// Makes the NPC walk towards the direction of the walkVector.
        /// Sets the vector to match the NPC's walk speed.
        /// </summary>
        /// <param name="walkVector">The direction to walk towards</param>
        public void Walk(Vector2 walkVector)
        {
            // Use the walk speed
            walkVector = walkVector.normalized * WalkSpeed;

            // Call WalkRaw() using the speed-adjusted vector
            WalkRaw(walkVector);
        }

        /// <summary>
        /// Makes the NPC walk towards the direction of the walkVector.
        /// Clamps the vector to the NPC's walk speed.
        /// </summary>
        /// <param name="walkVector">The direction to walk towards</param>
        public void WalkClamped(Vector2 walkVector)
        {
            // Clamp to walk speed
            walkVector = Vector2.ClampMagnitude(walkVector, WalkSpeed);

            // Call WalkRaw() using the clamped vector
            WalkRaw(walkVector);
        }

        /// <summary>
        /// Makes the NPC walk towards the direction of the walkVector.
        /// Assumes walkVector is in screen space and rotates it accordingly. Otherwise same functionality as Walk().
        /// </summary>
        /// <param name="walkVector">The direction to walk towards</param>
        public void WalkScreen(Vector2 walkVector)
        {
            // Rotate walk vector to match camera rotation
            walkVector = Quaternion.Euler(0, 0, 45) * walkVector;
            walkVector = Quaternion.Euler(0, 0, CameraController.Controller.Orientation.RotationAngle()) * walkVector;

            // Call Walk() using the rotated vector
            Walk(walkVector);
        }


        //// ---- AUXILIARY CONTROL FUNCTIONS ----

        /// <summary>
        /// Call to signal the end to the hitting motion
        /// </summary>
        public void EndHit()
        {
            State.Hitting = false;
        }


        //// ---- UPDATE FUNCTIONS ----

        protected override void OnUpdate()
        {
            VerifyFlip();
        }

        public override void InContainerUpdate()
        {
            
        }

        public void UpdateHand()
        {
            Sprite hand = null;
            if (Inventory != null)
            {
                if (Inventory.HandSlot != null)
                {
                    hand = Inventory.HandSlot.Entity.InventorySprite;
                }
            }

            HandSprite.sprite = hand;
        }

        /// <summary>
        /// Verify which direction the player should flip relative to the camera and its movement
        /// </summary>
        private void VerifyFlip()
        {
            Vector3 walk = Movement.WalkVector.normalized;

            if (walk.magnitude != 0)
            {
                float hor = CameraController.Controller.Orientation.RelativeHorizontal(walk);

                // Verify wether the player is moving horizontally, accounting for imprecisions during the rotation
                if (Mathf.Abs(hor) > 0.0001f)
                {
                    if (hor > 0 != Movement.Direction)
                    {
                        // Flip according to horizontal direction
                        Flip();
                    }
                }
            }
        }

        /// <summary>
        /// Gradually flip the player towards the direction
        /// </summary>
        private void Flip()
        {
            StartCoroutine(Flip(FlipDuration));
        }

        /// <summary>
        /// Instantly flip the player towards the direction
        /// </summary>
        private void InstantFlip()
        {
            StartCoroutine(Flip(0));
        }

        //// ---- LATEUPDATE FUNCTIONS ----

        void LateUpdate()
        {
            HandleAnimations();

            if (!Physics.IsInPhysicsMode)
            {
                // If the NPC is not under physics control, move by directly setting its velocity
                RigidBody.velocity = Movement.WalkVector;
            }
            //else
            //{
            //    // Else, movement applies force instead
            //    if (Vector2.Dot(RigidBodyComponent.velocity, NPCState.WalkVector) < 1)
            //    {
            //        RigidBodyComponent.AddForce(NPCState.WalkVector * RigidBodyComponent.mass, ForceMode2D.Impulse);
            //    }
            //}
        }

        private IEnumerator Flip(float duration)
        {
            float from = Movement.Direction ? 1 : -1;
            float to = -from;

            float current = from;
            float elapsed = 0;

            Movement.Direction = !Movement.Direction;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                current = Mathf.Lerp(from, to, elapsed / duration);

                Render.transform.localScale = new Vector3(current, 1, 1);

                yield return null;
            }

            Render.transform.localScale = new Vector3(to, 1, 1);
        }

        private void HandleAnimations()
        {
            if (Physics.IsInPhysicsMode)
            {
                // TODO: Special animation triggers for when in physics (ragdoll mode)
                Animator.SetBool("Hit", false);
                Animator.SetBool("Walk", false);
            }
            else
            {
                Animator.SetBool("Hit", State.Hitting);
                Animator.SetBool("Walk", Movement.WalkVector.magnitude != 0);
            }
        }



        public override void OnAddedToContainer(ContainerComponent container)
        {
            if (Brain != null)
            {
                Brain.SetContained(true);
            }

            Movement.WalkVector = Vector2.zero;
            base.OnAddedToContainer(container);
        }

        public override void OnRemovedFromContainer(ContainerComponent container)
        {
            if (Brain != null)
            {
                Brain.SetContained(false);
            }
            base.OnRemovedFromContainer(container);
        }
    }
}