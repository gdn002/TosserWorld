using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    public class TestBrain : BrainScript
    {
        private static readonly int AnimNoticeHash = Animator.StringToHash("PawnNotice");


        bool IsAware = false;

        Entity CurrentTarget = null;


        public override void RunBehaviorTree()
        {
            if (Me.IsChild)
                return;

            if (CurrentTarget != null)
            {
                if (IsAware)
                {
                    if (Me.DistanceTo(CurrentTarget) > 4 || WaitForAnimation(AnimNoticeHash))
                    {
                        if (Leash(CurrentTarget, 1, 2))
                        {
                            IsAware = false;
                        }
                    }

                    return;
                }

                if (Me.DistanceTo(CurrentTarget) > 2)
                {
                    PlayAnimation(AnimNoticeHash);
                    IsAware = true;
                    return;
                }

                if (Me.DistanceTo(CurrentTarget) > BrainComponent.AwarenessRadius)
                {
                    CurrentTarget = null;
                    return;
                }

                return;
            }

            CurrentTarget = Awareness.Find("Game Controller", true);
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
