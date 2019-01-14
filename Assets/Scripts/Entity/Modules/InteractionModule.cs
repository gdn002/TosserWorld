using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Interaction Module", menuName = "Modules/Interaction")]
    public class InteractionModule : Module
    {
        public enum Interactions
        {
            NoInteraction = 0,
            OpenContainer,
            PickUp,
            Equip,
        }

        public Interactions Interaction = Interactions.NoInteraction;
        public float InteractionRange { get; private set; }

        protected override void OnInitialize()
        {
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

        protected override Module Clone()
        {
            InteractionModule clone = CreateInstance<InteractionModule>();

            clone.Interaction = Interaction;

            return clone;
        }

        public bool IsInPickupRange(Entity activator)
        {
            // TODO: If we want different mobs to have different interaction ranges, review this
            return (Vector2.Distance(Owner.Position, activator.Position) < (InteractionRange + 0.5f));
        }

        public void RunInteraction(Entity activator)
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
            }
        }



        private void OpenContainerInteraction()
        {
            var container = Owner.GetModule<ContainerModule>();
            if (container != null)
            {
                container.OpenCloseContainer();
            }
        }

        private void PickUpInteraction(Entity activator)
        {
            var container = activator.GetModule<ContainerModule>();
            if (container != null)
            {
                container.Add(Owner);
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
