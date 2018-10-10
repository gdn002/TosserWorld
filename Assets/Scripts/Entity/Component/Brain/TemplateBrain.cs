using System.Collections;
using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brain
{
    // Template class for making NPC brains
    public class TemplateBrain : BrainComponent
    {
        // Declare any custom variables here
        int Value = 0;

        // The main loop runs every frame as long as the NPC is active - implement brain logic here
        // ALWAYS include this function, even if it must be empty
        protected override IEnumerator MainLoop()
        {
            // Typically you'll want to wrap the brain behavior in a while(true) loop
            while (true)
            {
                if (Value < 100)                    // Verify if Value is under 100
                {
                    CustomAction(1);                // Run CustomAction with parameter 1
                    yield return null;              // End frame
                }

                Talk("Done!");                      // Signal completion
                Value = 0;                          // Reset value to 0
                yield return new WaitForSeconds(5); // Wait for 5 seconds before starting over
            }
        }

        // The container loop runs while the NPC is inside a container - implement contained logic here
        // This function is OPTIONAL and can be ommited if no container behavior is neccessary
        protected override IEnumerator ContainerLoop()
        {
            yield return null;
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
