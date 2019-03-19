using TosserWorld.Entities;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    // Template class for making NPC brains
    public class DGBrain : BrainScript
    {
        private static readonly int AnimNoticeHash = Animator.StringToHash("PawnNotice");

        bool IsAware = false;
        Entity CurrentTarget = null;

        public override void RunBehaviorTree()
        {
            if (Me.IsChild)
                return;

            // If DG has a target, execute this block
            if (CurrentTarget != null)
            {
                // If DG is aware of a target, execute the "go to" block
                if (IsAware)
                {
                    // Give up if the target was picked up by something else
                    if (CurrentTarget.IsChild)
                    {
                        Stop();
                        CurrentTarget = null;
                        IsAware = false;
                        return;
                    }

                    // Wait until the "notice" animation has finished before going to the target
                    if (WaitForAnimation(AnimNoticeHash))
                    {
                        GoTo(CurrentTarget.Position);

                        // If in range, pick the target up
                        if (CurrentTarget.Interaction.RunInteraction(Me, Interactions.PickUp))
                        {
                            Stop();
                            IsAware = false;
                            CurrentTarget = null;
                        }
                    }
                }
                // If DG is not yet aware of the target, play the "notice" animation
                else
                {
                    Me.FlipTo(CurrentTarget.Position);
                    PlayAnimation(AnimNoticeHash);
                    IsAware = true;
                }
            }
            // Otherwise look for a target
            else
            {
                CurrentTarget = Awareness.FindNearest("Chicken Wing", false);
            }
        }
    }
}
