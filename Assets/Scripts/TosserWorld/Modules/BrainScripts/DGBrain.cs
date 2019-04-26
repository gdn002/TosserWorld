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
        Entity CurrentAgressor = null;

        public override void RunBehaviorTree()
        {
            if (Me.IsChild)
                return;

            // Check if DG has been attacked
            if (Triggers.Contains("Agressor"))
            {
                // Forget current target and remember the agressor
                CurrentAgressor = Triggers.Take<Entity>("Agressor");
                CurrentTarget = null;
                IsAware = false;
                ResetTimer();
            }

            // If DG has an agressor
            if (CurrentAgressor != null)
            {
                if (WaitForTimer(5))
                {
                    // If enough time has passed since the last attack, stop and forget
                    CurrentAgressor = null;
                    Stop();
                }
                else
                {
                    // Otherwise, flee
                    RunAwayFrom(CurrentAgressor.Position);
                    return;
                }
            }

            // If DG is hurt, check for healing items
            if (Me.Stats.Health.Maximum - Me.Stats.Health.Current >= 25)
            {
                var item = Me.Inventory.Search("Chicken Wing");
                if (item != null)
                {
                    // If found, use the item
                    item.Owner.ActivateAction(Me);
                }
            }

            // If DG has a target, execute this block
            if (CurrentTarget != null)
            {
                // Give up if the target was picked up by something else
                if (CurrentTarget.IsChild)
                {
                    CurrentTarget = null;
                    IsAware = false;
                    return;
                }

                // If DG is aware of a target, execute the "go to" block
                if (IsAware)
                {
                    // Wait until the "notice" animation has finished before going to the target
                    if (WaitForAnimation(AnimNoticeHash))
                    {
                        GoTo(CurrentTarget.Position);

                        // If in range, pick the target up
                        if (CurrentTarget.Interaction.RunInteraction(Me, Interactions.PickUp))
                        {
                            IsAware = false;
                            CurrentTarget = null;
                            ResetTimer();
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
                Stop();

                if (WaitForTimer(0.5f))
                {
                    CurrentTarget = Awareness.FindNearest("Chicken Wing", false);
                }
            }
        }
    }
}
