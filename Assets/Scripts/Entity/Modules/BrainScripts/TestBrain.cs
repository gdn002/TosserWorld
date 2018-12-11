using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    [CreateAssetMenu(fileName = "New Test Brain", menuName = "Modules/Brain Scripts/Test Brain")]
    public class TestBrain : BrainScript
    {
        protected override IEnumerator MainLoop()
        {
            if (!Me.RootMode)
                yield return null;

            if (Me.DistanceTo(PlayerEntity.Player) < 4)
            {
                Talk("Hi!");
                while (Me.DistanceTo(PlayerEntity.Player) < 4)
                {
                    Leash(PlayerEntity.Player, 1.5f, 3);
                    yield return null;
                }
            }
            else
            {
                GoTo(Vector2.zero);
                yield return null;
            }
        }
    }
}
