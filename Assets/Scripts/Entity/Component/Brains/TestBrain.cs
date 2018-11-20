using System.Collections;
using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brains
{
    [CreateAssetMenu(fileName = "New Test Brain", menuName = "Components/Brains/Test Brain")]
    public class TestBrain : Brain
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
