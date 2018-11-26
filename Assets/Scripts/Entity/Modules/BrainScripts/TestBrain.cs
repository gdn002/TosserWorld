using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    [CreateAssetMenu(fileName = "New Test Brain", menuName = "Modules/Brain Scripts/Test Brain")]
    public class TestBrain : BrainScript
    {
        protected override IEnumerator MainLoop()
        {
            if (Me.DistanceTo(PlayerEntity.Player) < 3)
            {
                Talk("Hi!");
                while (Me.DistanceTo(PlayerEntity.Player) < 3)
                {
                    Leash(PlayerEntity.Player, 0.5f, 1.5f);
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
