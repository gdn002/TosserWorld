using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TosserWorld.Entities;

namespace TosserWorld.Modules.BrainScripts
{
    public class BrainScriptSelector
    {
        private static List<string> Names = new List<string>();
        private static List<System.Type> Types = new List<System.Type>();

        static BrainScriptSelector()
        {
            // Empty Brain must ALWAYS be the first entry (zero index)
            BrainScript("Empty Brain", typeof(EmptyBrain));

            // All current BrainScript implementations must be included here
            BrainScript("Basic Follow Tosser", typeof(BasicFollowTosser));

            BrainScript("DG Brain", typeof(DGBrain));

            BrainScript("Tosser Brain", typeof(TosserBrain));
            BrainScript("Test Brain", typeof(TestBrain));
        }

        private static void BrainScript(string name, System.Type type)
        {
            Names.Add(name);
            Types.Add(type);
        }

        public static string Name(int i)
        {
            return Names[i];
        }

        public static string[] AllNames()
        {
            string[] allNames = new string[Names.Count];
            Names.CopyTo(allNames);
            return allNames;
        }

        public static BrainScript InstantiateScript(int i)
        {
            return System.Activator.CreateInstance(Types[i]) as BrainScript;
        }
    }

    public abstract class BrainScript
    {
        protected BrainModule BrainModule;

        protected Entity Me { get { return BrainModule.Owner; } }

        protected BrainModule.BrainTriggers Triggers { get { return BrainModule.Triggers; } }
        protected BrainModule.BrainAwareness Awareness { get { return BrainModule.Awareness; } }


        public void SetComponent(BrainModule component)
        {
            BrainModule = component;
        }


        public abstract void RunBehaviorTree();


        // ---- TIMER

        protected void ResetTimer()
        {
            BrainModule.Timer = 0;
        }

        protected bool WaitForTimer(float time)
        {
            return BrainModule.Timer >= time;
        }


        // ---- ANIMATION

        protected void PlayAnimation(int state)
        {
            if (Me.Animator != null)
                Me.Animator.Play(state);
        }

        protected bool WaitForAnimation(int state)
        {
            if (Me.Animator != null)
                return (Me.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash != state);

            return true;
        }

        protected void SetAnimation(string trigger, bool value)
        {
            if (Me.Animator != null)
                Me.Animator.SetBool(trigger, value);
        }


        // ---- MOVEMENT ACTIONS ----

        protected void Stop()
        {
            Me.Movement.Stop();
        }

        protected void Move(Vector2 direction)
        {
            Me.Movement.MoveFull(direction);
        }

        protected void MoveScreen(Vector2 direction)
        {
            Me.Movement.MoveScreenFull(direction);
        }

        protected bool GoTo(Vector2 destination)
        {
            Vector2 position = Me.Position;

            if (position != destination)
            {
                // Calculate the needed walk to reach the destination
                Vector2 direction = destination - position;

                if (direction.magnitude > Me.Movement.FrameLimit)
                {
                    // If the distance remaining is greater than the NPC's walk delta, walk at full speed
                    Move(direction);
                }
                else
                {
                    // If else, close the gap and stop movement
                    Me.Position = destination;
                    Stop();
                }

                return false;
            }

            return true;
        }

        protected bool Leash(Entity target, float near, float far)
        {
            Vector2 position = Me.Position;
            Vector2 destination = target.Position;

            // Calculate the needed walk to reach the target
            Vector2 direction = destination - position;
            if (direction.magnitude < near)
            {
                // Within stop threshold
                Stop();
                return true;
            }
            else if (Me.Movement.Movement.magnitude != 0 || direction.magnitude > far)
            {
                // Within go threshold
                Move(direction);
            }
            else if (Me.Movement.Movement.magnitude == 0)
            {
                return true;
            }

            return false;
        }

        protected bool RunAwayFrom(Vector2 point)
        {
            Vector2 direction = Me.Position - point;
            Move(direction);
            return true;
        }


        //// ---- OTHER ACTIONS ----

        protected void Talk(string line)
        {
            Me.StartCoroutine(BrainModule.Talk(line));
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
    }
}
