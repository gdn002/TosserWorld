using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    [CreateAssetMenu(fileName = "New Tosser Brain", menuName = "Modules/Brain Scripts/Tosser Brain")]
    public class TosserBrain : BrainScript
    {
        Vector2? Destination = null;
        Entity Target;

        protected override IEnumerator MainLoop()
        {
            if (Triggers.Contains("interact"))
            {
                Target = Triggers.Take("interact") as Entity;
                Destination = Target.Position;
            }
            else if (Triggers.Contains("player_walk_mouse"))                            // Verify mouse input
            {
                Destination = Triggers.Take<Vector2>("player_walk_mouse");              // Take the mouse destination
            }

            if (Triggers.Contains("player_walk_keyboard"))                              // Verify keyboard input
            {
                Destination = null;                                                     // Override destination
                MoveScreen(Triggers.Take<Vector2>("player_walk_keyboard"));             // Set player in motion according to input
            }
            else if (Destination != null)                                               // Verify destination
            {
                if (!GoTo(Destination.Value))                                           // Walk towards the destination
                {
                    Destination = null;                                                 // If the player arrived, clear destination
                }
                if (Target != null)
                {
                    if (Target.GetModule<InteractionModule>().IsInPickupRange(Me))
                    {
                        Destination = null;
                        Target.GetModule<InteractionModule>().RunInteraction(Me);
                        Target = null;
                    }
                }
            }
            else                                                                        // If there was no input or destination
            {
                Stop();                                                                 // Stop all movement
            }

            yield return null;                                                          // End frame
        }

        void PickupTest()
        {
            Entity item = Awareness.FindNearest();
            Me.EquipmentSlots[0].AddToSlot(item);
        }
    }
}
