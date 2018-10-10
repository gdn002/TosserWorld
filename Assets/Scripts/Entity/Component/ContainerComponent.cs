using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Entity.Type;

namespace Entity.Component
{
    public class ContainerComponent : MonoBehaviour
    {
        private static GameObject UIPrefab;
        private static Canvas UICanvas;


        public InventorySpace Storage;

        public int Rows = 3;
        public int Cols = 3;

        public int SlotCount { get { return Rows * Cols; } }

        private GameObject InventoryPanel;

        protected virtual void Awake()
        {
            if (UIPrefab == null)
            {
                UIPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/InventoryPanel");
            }

        }

        // Use this for initialization
        protected virtual void Start()
        {
            if (UICanvas == null)
            {
                UICanvas = GameObject.FindObjectOfType<Canvas>();
            }

            if (GetComponent<BaseEntity>() == null)
            {
                Debug.LogWarning("ContainerComponent in non-entity object: " + name);
            }

            Storage = new InventorySpace(SlotCount);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] != null)
                {
                    Storage[i].Entity.InContainerUpdate();
                }
            }
        }

        /// <summary>
        /// Adds a stack to the first available slot in the container.
        /// </summary>
        /// <param name="stack">The stack to add</param>
        /// <returns>False if there's no room left in the container, true otherwise</returns>
        public bool Add(EntityStack stack)
        {
            // First attempt to combine with an existing stack
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] != null)
                {
                    if (Storage[i].Entity.EntityName == stack.Entity.EntityName)
                    {
                        // Merge stacks
                        stack = Storage[i].CombineStack(stack);
                        if (stack == null)
                        {
                            // Stack was completely merged
                            return true;
                        }
                    }
                }
            }

            // If there's still leftover items, place them in the first empty slot
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] == null)
                {
                    // Put this stack in storage
                    Storage[i] = stack;
                    stack.Entity.OnAddedToContainer(this);
                    return true;
                }
            }

            // Container is out of space
            return false;
        }

        /// <summary>
        /// Places a stack in a container slot and returns the stack it replaced.
        /// </summary>
        /// <param name="stack">The stack to place</param>
        /// <param name="slot">The slot to place in</param>
        /// <returns>The stack that was previously in the slot (can be null)</returns>
        public EntityStack Place(EntityStack stack, int slot)
        {
            EntityStack current = Storage[slot];
            if (current != null)
            {
                if (current.Entity.EntityName == stack.Entity.EntityName)
                {
                    // Merge stacks
                    stack = Storage[slot].CombineStack(stack);

                    // Return whatever is left over from the merge
                    return stack;
                }
            }

            // Put stack in storage, replacing whatever was there
            Storage[slot] = stack;
            stack.Entity.OnAddedToContainer(this);

            return current;
        }

        /// <summary>
        /// Accesses a stack in a container slot.
        /// </summary>
        /// <param name="slot">The slot to access</param>
        /// <returns>The stack in the slot (can be null)</returns>
        public EntityStack Peek(int slot)
        {
            return Storage[slot];
        }

        /// <summary>
        /// Takes a stack from a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The stack taken (can be null)</returns>
        public EntityStack Take(int slot)
        {
            EntityStack current = Storage[slot];
            Storage[slot] = null;
            return current;
        }


        public void OpenCloseContainer()
        {
            if (InventoryPanel == null)
            {
                InventoryPanel = Instantiate(UIPrefab);
                InventoryPanel.GetComponent<UIInventory>().CreateInventoryGrid(this);
                InventoryPanel.transform.SetParent(UICanvas.transform, false);
            }
            else
            {
                Destroy(InventoryPanel);
                InventoryPanel = null;
            }
        }
    }
}