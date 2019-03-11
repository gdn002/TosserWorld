using System.Collections;
using UnityEngine;


namespace TosserWorld.Modules.BrainScripts
{
    // Template class for making NPC brains
    public class TemplateBrain : BrainScript
    {
        // Declare any custom variables here
        int Value = 0;

        // The main loop runs every frame as long as the NPC is active - implement brain logic here
        // ALWAYS include this function, even if it must be empty
        public override void RunBehaviorTree()
        {
            if (Value < 100)                    // Verify if Value is under 100
            {
                CustomAction(1);                // Run CustomAction with parameter 1
                return;                         // End frame
            }

            Talk("Done!");                      // Signal completion
            Value = 0;                          // Reset value to 0
        }


        // You can also write your own custom actions for this brain
        // As a standard, actions return a bool value - true if the action is considered "incomplete" and false if the action is considered "complete"
        bool CustomAction(int parameter)
        {
            // This action increases Value by parameter

            Value += parameter;
            return false;
        }
    }
}
