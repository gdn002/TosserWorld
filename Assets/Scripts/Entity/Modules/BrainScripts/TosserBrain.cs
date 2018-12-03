using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    [CreateAssetMenu(fileName = "New Tosser Brain", menuName = "Modules/Brain Scripts/Tosser Brain")]
    public class TosserBrain : BrainScript
    {
        Vector2? Destination = null;

        protected override IEnumerator MainLoop()
        {
            if (Triggers.Contains("player_walk_mouse"))                                 // Verify mouse input
            {
                Destination = Triggers.Take<Vector2>("player_walk_mouse");              // Take the mouse destination
            }

            if (Triggers.Contains("player_walk_keyboard"))                              // Verify keyboard input
            {
                Destination = null;                                                         // Override destination
                TriggerAnimation("Move");                                                   // Play movement animation
                MyMovement.MoveScreenFull(Triggers.Take<Vector2>("player_walk_keyboard"));  // Set player in motion according to input
            }
            else if (Destination != null)                                               // Verify destination
            {
                if (!GoTo(Destination.Value))                                           // Walk towards the destination
                {
                    Destination = null;                                                 // If the player arrived, clear destination
                }
            }
            else                                                                        // If there was no input or destination
            {
                Stop();                                                                 // Stop all movement
            }

            yield return null;                                                          // End frame
        }
    }
}
