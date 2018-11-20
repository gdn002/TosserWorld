using System.Collections;
using UnityEngine;


namespace Entity.Component.Brains
{
    [CreateAssetMenu(fileName = "New Empty Brain", menuName = "Components/Brains/Empty Brain")]
    public class EmptyBrain : Brain
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
