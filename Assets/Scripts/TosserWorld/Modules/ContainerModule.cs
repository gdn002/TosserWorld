using UnityEngine;

using TosserWorld.UI;
using TosserWorld.UI.Panels;
using TosserWorld.Entities;

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
    }

    [CreateAssetMenu(fileName = "New Container Configuration", menuName = "Modules/Container")]
    public class ContainerConfig : ModuleConfiguration
    {
        public int Rows = 3;
        public int Cols = 3;

        public int SlotCount { get { return Rows * Cols; } }
    }

    public class ContainerModule : Module
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

            // Verify for removed entities
            for (int i = 0; i < Storage.Length; i++)
            {
                if (Storage[i] != null)
                {
                    if (!Storage[i].Owner.Hierarchy.IsChildOf(Owner))
                        Storage[i] = null;
                }
            }
        }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            if (InventoryPrefab == null)
                InventoryPrefab = Resources.Load<GameObject>(DEFAULT_INVENTORY_PREFAB).GetComponent<UIInventory>();

            ContainerConfig containerConfig = configuration as ContainerConfig;
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
                    Owner.Hierarchy.AddChild(stack);
                    return true;
                }
            }

            // Container is out of space
            return false;
        }

        /// <summary>
        /// Places a stack in a container slot and returns the entity it replaced.
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
            Owner.Hierarchy.AddChild(stack);

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

        public void DropAll()
        {
            Vector2 dropPosition = Vector2.right;

            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] != null)
                {
                    Storage[i].Owner.Hierarchy.MakeIndependent(dropPosition);
                    Storage[i] = null;

                    dropPosition = Quaternion.Euler(0, 0, -15) * dropPosition;
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