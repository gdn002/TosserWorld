using UnityEngine;
using TosserWorld.Entities;
using TosserWorld.Modules.Configurations;

namespace TosserWorld.Modules
{
    public enum Interactions
    {
        NoInteraction = 0,
        OpenContainer,
        PickUp,
        Equip,
    }

    public class InteractionModule : Module
    {
        public Interactions Interaction;

        public float InteractionRange { get; private set; }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            InteractionConfig interactionConfig = configuration as InteractionConfig;
            Interaction = interactionConfig.Interaction;

            var circleCollider = Owner.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                InteractionRange = circleCollider.radius;
            }
            else
            {
                InteractionRange = 0;
            }
        }

        public bool IsInPickupRange(Entity activator)
        {
            // TODO: If we want different mobs to have different interaction ranges, review this
            return (Vector2.Distance(Owner.Position, activator.Position) < (InteractionRange + 0.5f));
        }

        public bool RunInteraction(Entity activator)
        {
            if (IsInPickupRange(activator))
            {
                switch(Interaction)
                {
                    case Interactions.OpenContainer:
                        OpenContainerInteraction();
                        break;

                    case Interactions.PickUp:
                        PickUpInteraction(activator);
                        break;

                    case Interactions.Equip:
                        EquipInteraction(activator);
                        break;
                }

                return true;
            }

            return false;
        }



        private void OpenContainerInteraction()
        {
            var container = Owner.Inventory;
            if (container != null)
            {
                container.OpenCloseContainer();
            }
        }

        private void PickUpInteraction(Entity activator)
        {
            var container = activator.Inventory;
            var stack = Owner.Stacking;

            if (container != null && stack != null)
            {
                container.Add(stack);
            }
        }

        private void EquipInteraction(Entity activator)
        {
            if (activator.EquipmentSlots.Length != 0)
            {
                activator.EquipmentSlots[0].AddToSlot(Owner);
            }
        }
    }
}
