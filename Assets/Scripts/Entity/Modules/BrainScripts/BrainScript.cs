using UnityEngine;
using System.Collections;

namespace TosserWorld.Modules.BrainScripts
{
    public abstract class BrainScript : ScriptableObject
    {
        public static BrainScript Clone(BrainScript brain)
        {
            return CreateInstance(brain.GetType()) as BrainScript;
        }

        private BrainModule BrainComponent;

        protected Entity Me { get { return BrainComponent.Owner; } }
        protected MovementModule MyMovement { get { return Me.GetModule<MovementModule>(); } }

        protected BrainModule.BrainTriggers Triggers { get { return BrainComponent.Triggers; } }
        protected BrainModule.BrainAwareness Awareness { get { return BrainComponent.Awareness; } }

        private Coroutine ActiveLoop = null;


        public void SetComponent(BrainModule component)
        {
            BrainComponent = component;
        }


        // ---- MAIN LOOPS ----

        public void StopActiveLoop()
        {
            if (ActiveLoop != null)
            {
                Me.StopCoroutine(ActiveLoop);
            }
        }

        public void RunMainLoop()
        {
            StopActiveLoop();
            ActiveLoop = Me.StartCoroutine(InternalMainLoop());
        }

        public void RunContainerLoop()
        {
            StopActiveLoop();
            ActiveLoop = Me.StartCoroutine(InternalContainerLoop());
        }


        protected abstract IEnumerator MainLoop();
        protected virtual IEnumerator ContainerLoop()
        {
            yield return null;
        }


        public void TriggerAnimation(string trigger)
        {
            if (Me.Animator != null)
                Me.Animator.SetTrigger(trigger);
        }

        //// ---- MAIN ACTIONS ----

        protected void Stop()
        {
            TriggerAnimation("Stop");
            MyMovement.Stop();
        }

        protected void Move(Vector2 direction)
        {
            Me.FlipTo(direction);
            MyMovement.MoveFull(direction);
            TriggerAnimation("Move");
        }

        protected void MoveScreen(Vector2 direction)
        {
            Me.FlipToScreen(direction);
            MyMovement.MoveScreenFull(direction);
            TriggerAnimation("Move");
        }

        protected bool GoTo(Vector2 destination)
        {
            Vector2 position = Me.transform.position;

            if (position != destination)
            {
                // Calculate the needed walk to reach the destination
                Vector2 direction = destination - position;

                if (direction.magnitude > MyMovement.FrameLimit)
                {
                    // If the distance remaining is greater than the NPC's walk delta, walk at full speed
                    Move(direction);
                }
                else
                {
                    // If else, close the gap and stop movement
                    Me.transform.position = destination;
                    Stop();
                }

                return false;
            }

            return true;
        }

        protected bool Leash(Entity target, float near, float far)
        {
            Vector2 position = Me.transform.position;
            Vector2 destination = target.transform.position;

            // Calculate the needed walk to reach the target
            Vector2 direction = destination - position;
            if (direction.magnitude < near)
            {
                // Within stop threshold
                Stop();
            }
            else if (MyMovement.Movement.magnitude != 0 || direction.magnitude > far)
            {
                // Within go threshold
                Move(direction);
            }
            else if (MyMovement.Movement.magnitude == 0)
            {
                TriggerAnimation("Stop");
            }

            return true;
        }


        //// ---- SECONDARY ACTIONS ----

        protected void Talk(string line)
        {
            Me.StartCoroutine(BrainComponent.Talk(line));
        }


        //// ---- EVENTS ----

        public virtual void OnDetectEntity(Entity entity)
        {
            //Debug.Log("Detected: " + entity.name);
        }

        public virtual void OnLoseEntity(Entity entity)
        {
            //Debug.Log("Lost: " + entity.name);
        }


        //// ---- INTERNAL COROUTINES ----

        private IEnumerator InternalMainLoop()
        {
            while (true)
            {
                yield return MainLoop();
            }
        }

        private IEnumerator InternalContainerLoop()
        {
            while (true)
            {
                yield return ContainerLoop();
            }
        }
    }
}
