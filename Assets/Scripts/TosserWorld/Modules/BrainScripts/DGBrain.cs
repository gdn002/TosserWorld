using TosserWorld.Entities;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    // Template class for making NPC brains
    public class DGBrain : BrainScript
    {
        private static readonly int AnimNoticeHash = Animator.StringToHash("PawnNotice");

        bool IsAware = false;
        Entity Focus = null;
        Entity Aggressor = null;


        public override void RunBehaviorTree()
        {
            if (Me.IsChild)
                return;

            // Check if DG has been attacked
            if (Triggers.Contains("Agressor"))
            {
                // Forget current focus and remember the agressor
                ClearStatus();

                Aggressor = Triggers.Take<Entity>("Agressor");
                ResetTimer();
            }

            // If DG has an aggressor
            if (Aggressor != null)
            {
                // Execute combat routine
                CombatRoutine();
                return;
            }

            // If DG is hurt
            if (Me.Stats.Health.PercentAt < 0.7f)
            {
                // Execute healing routine
                HealingRoutine();
                return;
            }

            // If there's nothing else to do, execute looking for food routine
            LookForFoodRoutine();
        }


        private void ClearStatus()
        {
            Aggressor = null;
            Focus = null;
            IsAware = false;
        }


        private void CombatRoutine()
        {
            // If enough time has passed since the last attack
            // Or the aggressor is dead
            if (WaitForTimer(5) || !Aggressor.IsAlive)
            {
                // Stop and forget
                ClearStatus();
                Stop();
                return;
            }

            // If already has a weapon
            if (Me.EquipmentSlots[0].Equipped != null && Me.EquipmentSlots[0].Equipped.TagList.HasTag(EntityTags.Weapon))
            {
                // Move towards the aggressor
                GoTo(Aggressor.Position);

                // If close enough
                if (Me.DistanceTo(Aggressor) < 1.5f)
                {
                    // Use the weapon
                    Me.ActivateEquipment();
                }

                return;
            }

            // If a weapon was found
            if (Focus != null)
            {
                // Give up if the weapon was picked up by something else
                if (Focus.IsChild)
                {
                    Focus = null;
                    return;
                }

                // Move towards the weapon
                GoTo(Focus.Position);

                // If in range, equip the weapon
                if (Focus.Interaction.RunInteraction(Me, Interactions.Equip))
                {
                    Focus = null;
                }

                return;
            }
            // If no weapon was found yet
            else
            {
                // Look for a nearby weapon
                Focus = Awareness.FindNearest(EntityTags.Weapon);
            }

            // If no weapon can be found, flee
            RunAwayFrom(Aggressor.Position);
        }

        private void HealingRoutine()
        {
            // Check if already has a healing item
            var item = Me.Inventory.Search(EntityTags.Healing);
            if (item != null)
            {
                // Use the item
                item.Owner.ActivateAction(Me);
            }
            else
            {
                // If has found a healing item already
                if (Focus != null && Focus.TagList.HasTag(EntityTags.Healing))
                {
                    GoTo(Focus.Position);

                    // If in range, pick the item up
                    if (Focus.Interaction.RunInteraction(Me, Interactions.PickUp))
                    {
                        ClearStatus();
                    }
                }
                // If hasn't found a healing item yet
                else
                {
                    Focus = Awareness.FindNearest(EntityTags.Healing);
                }
            }
        }

        private void LookForFoodRoutine()
        {
            // If already found food
            if (Focus != null)
            {
                // Give up if the target was picked up by something else
                if (Focus.IsChild)
                {
                    ClearStatus();
                    return;
                }

                // If aware of the food
                if (IsAware)
                {
                    // Wait until the "notice" animation has finished before chasing the food
                    if (WaitForAnimation(AnimNoticeHash))
                    {
                        GoTo(Focus.Position);

                        // If in range, pick the food up
                        if (Focus.Interaction.RunInteraction(Me, Interactions.PickUp))
                        {
                            ClearStatus();
                            ResetTimer();
                            return;
                        }
                    }
                }
                // If not yet aware of the food, play the "notice" animation
                else
                {
                    Me.FlipTo(Focus.Position);
                    PlayAnimation(AnimNoticeHash);
                    IsAware = true;
                }
            }
            // If hasn't found food already
            else
            {
                Stop();

                // Look for food after the grace period has passed
                if (WaitForTimer(0.5f))
                {
                    Focus = Awareness.FindNearest(EntityTags.Food);
                }
            }
        }
    }
}
