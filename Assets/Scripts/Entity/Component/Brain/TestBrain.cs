using System.Collections;
using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brain
{
    public class TestBrain : BrainComponent
    {
        protected override IEnumerator MainLoop()
        {
            if (Owner.DistanceTo(PlayerEntity.Player) < 3)
            {
                Talk("Hi!");
                while (Owner.DistanceTo(PlayerEntity.Player) < 3)
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
