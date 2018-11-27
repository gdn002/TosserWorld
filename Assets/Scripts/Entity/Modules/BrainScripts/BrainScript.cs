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

        private Brain BrainComponent;

        protected Entity Me { get { return BrainComponent.Owner; } }
        protected Mover MyMovement { get { return Me.GetModule<Mover>(); } }

        protected Brain.BrainTriggers Triggers { get { return BrainComponent.Triggers; } }
        protected Brain.BrainAwareness Awareness { get { return BrainComponent.Awareness; } }

        private Coroutine ActiveLoop = null;


        public void SetComponent(Brain component)
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


        //// ---- MAIN ACTIONS ----

        protected bool GoTo(Vector2 destination)
        {
            Vector2 position = Me.transform.position;

            if (position != destination)
            {
                // Calculate the needed walk to reach the destination
                Vector2 walk = destination - position;

                if (walk.magnitude > MyMovement.FrameLimit)
                {
                    // If the distance remaining is greater than the NPC's walk delta, walk at full speed
                    MyMovement.MoveFull(walk);
                }
                else
                {
                    // If else, close the gap and stop movement
                    Me.transform.position = destination;
                    MyMovement.Stop();
                }

                return true;
            }

            return false;
        }

        protected bool Leash(Entity target, float near, float far)
        {
            Vector2 position = Me.transform.position;
            Vector2 destination = target.transform.position;

            // Calculate the needed walk to reach the target
            Vector2 walk = destination - position;
            if (walk.magnitude < near)
            {
                // Within stop threshold
                MyMovement.Stop();
            }
            else if (MyMovement.Movement.magnitude != 0 || walk.magnitude > far)
            {
                // Within go threshold
                MyMovement.MoveFull(walk);
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
