using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Entity.Type;
using System.Collections;

namespace Entity.Component.Brains
{
    public abstract class Brain : ScriptableObject
    {
        public static Brain Clone(Brain brain)
        {
            return CreateInstance(brain.GetType()) as Brain;
        }

        private NPCBrain BrainComponent;

        protected NPCEntity Me { get { return BrainComponent.Owner; } }
        protected NPCBrain.BrainTriggers Triggers { get { return BrainComponent.Triggers; } }
        protected NPCBrain.BrainAwareness Awareness { get { return BrainComponent.Awareness; } }

        private Coroutine ActiveLoop = null;


        public void SetComponent(NPCBrain component)
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

                if (walk.magnitude > Me.Stats.FrameSpeed)
                {
                    // If the distance remaining is greater than the NPC's walk delta, walk at full speed
                    Me.Walk(walk);
                }
                else
                {
                    // If else, close the gap and stop movement
                    Me.transform.position = destination;
                    Me.Stop();
                }

                return true;
            }

            return false;
        }

        protected bool Leash(BasicEntity target, float near, float far)
        {
            Vector2 position = Me.transform.position;
            Vector2 destination = target.transform.position;

            // Calculate the needed walk to reach the target
            Vector2 walk = destination - position;
            if (walk.magnitude < near)
            {
                // Within stop threshold
                Me.Stop();
            }
            else if (Me.Movement.WalkVector.magnitude != 0 || walk.magnitude > far)
            {
                // Within go threshold
                Me.Walk(walk);
            }

            return true;
        }


        //// ---- SECONDARY ACTIONS ----

        protected void Talk(string line)
        {
            Me.StartCoroutine(BrainComponent.Talk(line));
        }


        //// ---- EVENTS ----

        public virtual void OnDetectEntity(BasicEntity entity)
        {
            //Debug.Log("Detected: " + entity.name);
        }

        public virtual void OnLoseEntity(BasicEntity entity)
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
