using UnityEngine;
using TosserWorld.Entities;
using TosserWorld.Modules.Configurations;

namespace TosserWorld.Modules
{
    public enum Interactions
    {
        NoInteraction = 0,
        OpenInventory,
        PickUp,
        Equip,
    }

    public class InteractionModule : Module
    {
        public Interactions DefaultInteraction;
        public Interactions DefaultDeadInteraction;

        public float InteractionRange { get; private set; }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            InteractionConfig interactionConfig = configuration as InteractionConfig;
            DefaultInteraction = interactionConfig.DefaultInteraction;
            DefaultDeadInteraction = interactionConfig.DefaultDeadInteraction;

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

        public bool IsInInteractingRange(Entity activator)
        {
            // TODO: If we want different mobs to have different interaction ranges, review this
            return (Vector2.Distance(Owner.Position, activator.Position) < (InteractionRange + 0.5f));
        }

        public Interactions CurrentDefaultInteraction()
        {
            if (Owner.IsAlive)
            {
                return DefaultInteraction;
            }
            else
            {
                return DefaultDeadInteraction;
            }
        }

        /// <summary>
        /// Run the current default interaction between the activator and this entity.
        /// </summary>
        /// <param name="activator">The entity activating the interaction.</param>
        /// <returns></returns>
        public bool RunInteraction(Entity activator)
        {
            if (IsInInteractingRange(activator))
            {
                switch(CurrentDefaultInteraction())
                {
                    case Interactions.OpenInventory:
                        OpenInventoryInteraction();
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

        /// <summary>
        /// Run a specific interaction between the activator and this entity
        /// </summary>
        /// <param name="activator">The entity activating the interaction.</param>
        /// <param name="interaction">The interaction to run.</param>
        /// <returns></returns>
        public bool RunInteraction(Entity activator, Interactions interaction)
        {
            if (IsInInteractingRange(activator))
            {
                switch (interaction)
                {
                    case Interactions.OpenInventory:
                        OpenInventoryInteraction();
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


        public bool CanRunInteraction(Entity activator, Interactions interaction)
        {
            switch (interaction)
            {
                case Interactions.OpenInventory:
                    return CanOpenInventory();

                case Interactions.PickUp:
                    return CanPickUp(activator);

                case Interactions.Equip:
                    return CanEquip(activator);
            }

            return false;
        }



        private bool CanOpenInventory()
        {
            return Owner.Inventory != null;
        }

        private void OpenInventoryInteraction()
        {
            if (CanOpenInventory())
            {
                Owner.Inventory.OpenCloseContainer();
            }
        }


        private bool CanPickUp(Entity activator)
        {
            return activator.Inventory != null && Owner.Stacking != null;
        }

        private void PickUpInteraction(Entity activator)
        {
            if (CanPickUp(activator))
            {
                activator.Inventory.Add(Owner.Stacking);
            }
        }


        private bool CanEquip(Entity activator)
        {
            return activator.EquipmentSlots.Length != 0;
        }

        private void EquipInteraction(Entity activator)
        {
            if (CanEquip(activator))
            {
                activator.EquipmentSlots[0].AddToSlot(Owner);
            }
        }
    }
}
