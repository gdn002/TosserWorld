using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;

namespace Entity.Component
{
    public class InventoryComponent : ContainerComponent
    {
        public EntityStack HandSlot;


        private NPCEntity Entity;

        protected override void Awake()
        {
            base.Awake();
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            Entity = GetComponent<NPCEntity>();
            if (Entity == null)
            {
                Debug.LogWarning("InventoryComponent in non-NPC entity: " + name);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }


        public EntityStack PlaceInHand(EntityStack stack)
        {
            EntityStack current = HandSlot;

            if (current != null)
            {
                if (current.Entity.EntityName == stack.Entity.EntityName)
                {
                    // Merge stacks
                    stack = current.CombineStack(stack);

                    // Return whatever is left of the stack
                    return stack;
                }
            }

            // Replace current stack in hand
            HandSlot = stack;
            stack.Entity.OnAddedToContainer(this);
            UpdateHand();

            // Return replaced stack
            return current;
        }

        public EntityStack TakeFromHand()
        {
            EntityStack current = HandSlot;
            HandSlot = null;
            UpdateHand();
            return current;
        }

        private void UpdateHand()
        {
            Entity.UpdateHand();
        }
    }
}
