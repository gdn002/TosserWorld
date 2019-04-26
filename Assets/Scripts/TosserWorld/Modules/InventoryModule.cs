using UnityEngine;

using TosserWorld.UI;
using TosserWorld.UI.Panels;
using TosserWorld.Entities;
using TosserWorld.Modules.Configurations;
using System.Collections.Generic;

namespace TosserWorld.Modules
{
    public class InventorySpace
    {
        public StackingModule[] Inventory;

        public int Length { get { return Inventory.Length; } }

        public InventorySpace(int size)
        {
            Inventory = new StackingModule[size];
        }

        public StackingModule this[int index]
        {
            get
            {
                return Inventory[index];
            }
            set
            {
                Inventory[index] = value;
            }
        }

        public IEnumerator<StackingModule> GetEnumerator()
        {
            foreach(var item in Inventory)
            {
                yield return item;
            }
        }

        public IEnumerable<StackingModule> GetContents()
        {
            foreach (var item in Inventory)
            {
                if (item != null)
                    yield return item;
            }
        }
    }

    /// <summary>
    /// Module for giving the entity an inventory space.
    /// </summary>
    public class InventoryModule : Module
    {
        private static string DEFAULT_INVENTORY_PREFAB = "Prefabs/UI/Inventory/InventoryPanel";

        public int Rows;
        public int Cols;
        public int SlotCount { get { return Rows * Cols; } }

        public UIInventory InventoryPrefab;
        public InventorySpace Storage;

        private UIInventory InventoryPanel;


        public override void Update()
        {
            if (InventoryPanel != null)
            {
                // If the container is open and the player moves far enough away from it, close it
                if (Vector2.Distance(PlayerEntity.Player.Position, Owner.Position) > 1.5f)
                {
                    OpenCloseContainer();
                }
            }
        }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            if (InventoryPrefab == null)
                InventoryPrefab = Resources.Load<GameObject>(DEFAULT_INVENTORY_PREFAB).GetComponent<UIInventory>();

            InventoryConfig containerConfig = configuration as InventoryConfig;
            Rows = containerConfig.Rows;
            Cols = containerConfig.Cols;

            Storage = new InventorySpace(SlotCount);
        }

        /// <summary>
        /// Adds a stack to the first available slot in the container.
        /// </summary>
        /// <param name="stack">The stack to add</param>
        /// <returns>False if there's no room left in the container, true otherwise</returns>
        public bool Add(StackingModule stack)
        {
            // First attempt to combine with an existing stack
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (stack.StacksMatch(Storage[i]))
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

            // If there's still leftover items, place them in the first empty slot
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] == null)
                {
                    // Put this entity in storage
                    Storage[i] = stack;
                    stack.Owner.SetParent(Owner);
                    stack.Owner.Hide();
                    return true;
                }
            }

            // Container is out of space
            return false;
        }

        /// <summary>
        /// Places a stack in a container slot and returns the stack it replaced, if any.
        /// </summary>
        /// <param name="stack">The stack to place</param>
        /// <param name="slot">The slot to place in</param>
        /// <returns>The stack that was previously in the slot (may be null)</returns>
        public StackingModule Place(StackingModule stack, int slot)
        {
            StackingModule current = Storage[slot];
            if (stack.StacksMatch(current))
            {
                // Merge stacks
                stack = Storage[slot].CombineStack(stack);

                // Return whatever is left over from the merge
                return stack;
            }

            // Put entity in storage, replacing whatever was there
            Storage[slot] = stack;
            stack.Owner.SetParent(Owner);
            stack.Owner.Hide();

            return current;
        }

        /// <summary>
        /// Accesses a stack in a container slot.
        /// </summary>
        /// <param name="slot">The slot to access</param>
        /// <returns>The stack in the slot (may be null)</returns>
        public StackingModule Peek(int slot)
        {
            return Storage[slot];
        }

        /// <summary>
        /// Searches this inventory for an entity that matches a given name.
        /// </summary>
        /// <param name="name">The name to search</param>
        /// <returns>The first stored stack that matches the name (may be null)</returns>
        public StackingModule Search(string name)
        {
            foreach (var item in Storage)
            {
                if (item != null)
                {
                    if (item.Owner.Name == name)
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Takes a stack from a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The stack taken (may be null)</returns>
        public StackingModule Take(int slot)
        {
            StackingModule current = Storage[slot];
            Storage[slot] = null;
            return current;
        }

        /// <summary>
        /// Takes a single unit from a stack in a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The unit taken (may be null)</returns>
        public StackingModule TakeSingle(int slot)
        {
            StackingModule current = Storage[slot];
            if (current != null)
            {
                if (current.Amount > 1)
                {
                    return current.TakeFromStack(1);
                }
            }

            return Take(slot);
        }

        /// <summary>
        /// Takes half the units from a stack in a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The units taken (may be null)</returns>
        public StackingModule TakeHalf(int slot)
        {
            StackingModule current = Storage[slot];
            if (current != null)
            {
                if (current.Amount > 1)
                {
                    return current.TakeFromStack(current.Amount / 2);
                }
            }

            return Take(slot);
        }

        /// <summary>
        /// Searches for a stack and removes it from this inventory if found. 
        /// Use this only when the stack is being moved out of the inventory externally to "clean up" its inventory slot.
        /// </summary>
        /// <param name="item">The stack to remove</param>
        /// <returns>True if the stack was found in the storage and removed (or if it was null), false otherwise.</returns>
        public bool Remove(StackingModule item)
        {
            if (item == null)
                return true;

            for (int i = 0; i < Storage.Length; ++i)
            {
                if(Storage[i] == item)
                {
                    Storage[i] = null;
                    return true;
                }
            }

            return false;
        }

        public void DropAll()
        {
            Vector2 drop = Vector2.right;

            for (int i = 0; i < Storage.Length; ++i)
            {
                StackingModule item = Storage[i];

                if (item != null)
                {
                    item.Owner.SetParent(null);
                    item.Owner.Position = Storage[i].Owner.Position + drop;
                    item.Owner.Hide(false);

                    drop = Quaternion.Euler(0, 0, -15) * drop;
                }
            }
        }

        public void OpenCloseContainer()
        {
            if (InventoryPanel == null)
            {
                InventoryPanel = UIPanel.InstantiatePanel(InventoryPrefab) as UIInventory;
                InventoryPanel.CreateInventoryGrid(this);
                UIManager.Manager.AddPanel(InventoryPanel);
            }
            else
            {
                UIManager.Manager.RemovePanel(InventoryPanel);
                InventoryPanel = null;
            }
        }
    }
}