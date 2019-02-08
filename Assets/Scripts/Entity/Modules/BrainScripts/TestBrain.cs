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
                        GoTo(CurrentTarget.Position);
                        if (CurrentTarget.GetModule<InteractionModule>().RunInteraction(Me))
                        {
                            Stop();
                            IsAware = false;
                            CurrentTarget = null;
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

            CurrentTarget = Awareness.Find("KFC", true);
        }
    }
}
