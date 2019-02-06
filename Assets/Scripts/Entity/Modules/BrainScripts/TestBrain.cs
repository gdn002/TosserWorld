using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    public class TestBrain : BrainScript
    {
        protected override IEnumerator MainLoop()
        {
            if (Me.IsChild)
                yield return null;

            Entity gameController = Awareness.Find("Game Controller", true);

            if (gameController != null)
            {
                if (Me.DistanceTo(gameController) > 2)
                    yield return PlayAnimation("PawnNotice");

                while (Leash(gameController, 1, 2))
                {
                    yield return null;
                }
            }
        }

        /*
        protected override IEnumerator MainLoop()
        {
            if (Me.SubEntity)
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
        */
    }
}
