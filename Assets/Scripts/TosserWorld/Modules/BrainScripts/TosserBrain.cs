using System.Collections;
using TosserWorld.UI;
using UnityEngine;

using TosserWorld.Entities;

namespace TosserWorld.Modules.BrainScripts
{
    public class TosserBrain : BrainScript
    {
        public class LocalTriggers
        {
            public static string DROP_CURSOR    = "player_drop_cursor";
            public static string INTERACT       = "player_interact";

            public static string WALK_MOUSE     = "player_walk_mouse";
            public static string WALK_KEYBOARD  = "player_walk_keyboard";

        }

        Vector2? Destination = null;
        Entity Target;

        // Tasks
        bool DropCursor = false;

        public void ClearAllTasks()
        {
            DropCursor = false;
        }


        public override void RunBehaviorTree()
        {
            if (Triggers.Contains(LocalTriggers.INTERACT))                              // If tosser was ordered to interact with something...
            {
                ClearAllTasks();                                                        // ...clear all tasks...
                Target = Triggers.Take(LocalTriggers.INTERACT) as Entity;               // ...find out what the target is...
                Destination = Target.Position;                                          // ...then set their destination to the target's position.
            }
            else if (Triggers.Contains(LocalTriggers.DROP_CURSOR))                      // If tosser was ordered to drop something instead...
            {
                DropCursor = true;                                                      // ...let they know they're on their way to drop something...
                Destination = Triggers.Take<Vector2>(LocalTriggers.DROP_CURSOR);        // ... and set their destination to the clicked spot in the world.
            }
            else if (Triggers.Contains(LocalTriggers.WALK_MOUSE))                       // If tosser was ordered to just walk via mouse click instead...
            {
                ClearAllTasks();                                                        // ...clear all tasks...
                Destination = Triggers.Take<Vector2>(LocalTriggers.WALK_MOUSE);         // ...set their destination to the clicked spot in the world.
            }

            if (Triggers.Contains(LocalTriggers.WALK_KEYBOARD))                         // If tosser was ordered to walk via keyboard...
            {
                ClearAllTasks();                                                        // ...clear all tasks...
                Destination = null;                                                     // ...override their current destination...
                MoveScreen(Triggers.Take<Vector2>(LocalTriggers.WALK_KEYBOARD));        // ...and move accordingly.
            }
            else if (Destination != null)                                               // If tosser has a set destination instead...
            {
                bool arrived = GoTo(Destination.Value);                                 // ...walk towards the destination.

                if (Target != null)                                                     // If tosser has an interaction target...
                {
                    if (Target.Interaction.IsInInteractingRange(Me))                         // ...if they are in range...
                    {
                        arrived = true;                                                 // ...if they are, tell them they arrived,
                        Target.Interaction.RunInteraction(Me);                          // run the interaction,
                        Target = null;                                                  // and clear the target as well.
                    }
                }
                else if (DropCursor)                                                    // If tosser has to drop something...
                {
                    if (Vector2.Distance(Me.Position, Destination.Value) <= 0.5f)       // ...if they're close enough...
                    {
                        arrived = true;                                                 // ...tell them they arrived,
                        UICursor.Cursor.DropAttachedEntity(Destination.Value);          // and drop the cursor entity.
                    }
                }

                if (arrived)                                                            // If tosser arrived at his destination...
                {
                    Destination = null;                                                 // ...clear it.
                }
            }
            else                                                                        // If there was no input or destination at all...
            {
                Stop();                                                                 // ...stop all movement
            }
        }
    }
}
