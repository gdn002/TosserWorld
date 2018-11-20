using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Component;
using Entity.Component.Brains;

namespace Entity.Type
{
    /// <summary>
    /// ENTITY CLASS FOR ALL NPCS
    /// Handles NPC movement and interactions
    /// BasicEntity -> NPCEntity
    /// </summary>
    public class NPCEntity : BasicEntity
    {
        public NPCBrain Brain;
        public NPCStats Stats;

        // NPC movement
        public class NPCMovement
        {
            public NPCMovement()
            {
                Direction = true;
            }

            public bool Knockback;

            public bool Direction;      // true = right, false = left
            public Vector2 WalkVector;  // Walking velocity
        }
        public NPCMovement Movement = new NPCMovement();

        // NPC components
        public struct NPCEntityComponents
        {
            // Hierarchy
            public SpriteRenderer HandSprite;
        }
        public NPCEntityComponents NPCComponents;

        // Initialization
        protected override void Start()
        {
            // Detect and fill Entity component references
            NPCComponents.HandSprite = transform.Find("Render").Find("Body").Find("Hand").GetComponentInChildren<SpriteRenderer>();

            // Empty components get default values
            Brain = BasicComponent.LoadTemplate(Brain);
            Stats = BasicComponent.LoadTemplate(Stats);

            Brain.Initialize(this);
            Stats.Initialize(this);

            base.Start();
        }

        protected override void Update()
        {
            VerifyFlip();

            base.Update();
        }

        protected override void LateUpdate()
        {
            if (IsInitialized)
            {
                base.LateUpdate();

                Animate();

                if (!Physics.Active)
                {
                    // If the NPC is not under physics control, move by directly setting its velocity
                    Components.RigidBody.velocity = Movement.WalkVector;
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
            walkVector = walkVector.normalized * Stats.Speed;

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
            walkVector = Vector2.ClampMagnitude(walkVector, Stats.Speed);

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


        //// ---- UPDATE FUNCTIONS ----

        public void UpdateHand()
        {
            // TODO
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
            StartCoroutine(Flip(0.1f));
        }

        /// <summary>
        /// Instantly flip the player towards the direction
        /// </summary>
        private void InstantFlip()
        {
            StartCoroutine(Flip(0));
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

                Components.Render.transform.localScale = new Vector3(current, 1, 1);

                yield return null;
            }

            Components.Render.transform.localScale = new Vector3(to, 1, 1);
        }

        private void Animate()
        {
            if (Physics.Active)
            {
                // TODO: Special animation triggers for when in physics (ragdoll mode)
                Components.Animator.SetBool("Hit", false);
                Components.Animator.SetBool("Walk", false);
            }
            else
            {
                //Components.Animator.SetBool("Hit", State.Hitting);
                Components.Animator.SetBool("Walk", Movement.WalkVector.magnitude != 0);
            }
        }



        public override void OnAddedToContainer(BasicInventory inventory)
        {
            if (Brain != null) Brain.SetContained(true);

            Movement.WalkVector = Vector2.zero;
            base.OnAddedToContainer(inventory);
        }

        public override void OnRemovedFromContainer(BasicInventory inventory)
        {
            if (Brain != null) Brain.SetContained(false);
            base.OnRemovedFromContainer(inventory);
        }
    }
}