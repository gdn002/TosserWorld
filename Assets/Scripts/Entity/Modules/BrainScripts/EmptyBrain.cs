using System.Collections;
using UnityEngine;


namespace TosserWorld.Modules.BrainScripts
{
    public class EmptyBrain : BrainScript
    {
        protected override IEnumerator MainLoop()
        {
            while (true)
            {
                // Basically does nothing
                yield return null;
            }
        }
    }
}
