using System.Collections;
using UnityEngine;


namespace TosserWorld.Modules.BrainScripts
{
    [CreateAssetMenu(fileName = "New Empty Brain", menuName = "Modules/Brain Scripts/Empty Brain")]
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
