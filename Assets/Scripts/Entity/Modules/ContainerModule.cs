using UnityEngine;

using TosserWorld.UI;
using TosserWorld.UI.Panels;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Container Module", menuName = "Modules/Container")]
    public class ContainerModule : Module
    {
        private static string DEFAULT_INVENTORY_PREFAB = "Prefabs/UI/Inventory/InventoryPanel";

        public UIInventory InventoryPrefab;
        public InventorySpace Storage;

        public int Rows;
        public int Cols;

        public int SlotCount { get { return Rows * Cols; } }

        private UIInventory InventoryPanel;


        public ContainerModule()
        {
            Rows = 3;
            Cols = 3;
        }

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
                    if (!Storage[i].Hierarchy.IsChildOf(Owner))
                        Storage[i] = null;
                }
            }
        }

        protected override Module Clone()
        {
            ContainerModule clone = CreateInstance<ContainerModule>();

            clone.Rows = Rows;
            clone.Cols = Cols;

            return clone;
        }

        protected override void OnInitialize()
        {
            if (InventoryPrefab == null)
                InventoryPrefab = Resources.Load<GameObject>(DEFAULT_INVENTORY_PREFAB).GetComponent<UIInventory>();

            Storage = new InventorySpace(SlotCount);
        }

        /// <summary>
        /// Adds an entity to the first available slot in the container.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>False if there's no room left in the container, true otherwise</returns>
        public bool Add(Entity entity)
        {
            // First attempt to combine with an existing stack
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (entity.MatchStacks(Storage[i]))
                {
                    // Merge stacks
                    entity = Storage[i].GetModule<StackingModule>().CombineStack(entity);
                    if (entity == null)
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
                    Storage[i] = entity;
                    Owner.Hierarchy.AddChild(entity);
                    return true;
                }
            }

            // Container is out of space
            return false;
        }

        /// <summary>
        /// Places an entity in a container slot and returns the entity it replaced.
        /// </summary>
        /// <param name="entity">The entity to place</param>
        /// <param name="slot">The slot to place in</param>
        /// <returns>The entity that was previously in the slot (may be null)</returns>
        public Entity Place(Entity entity, int slot)
        {
            Entity current = Storage[slot];
            if (entity.MatchStacks(current))
            {
                // Merge stacks
                entity = Storage[slot].GetModule<StackingModule>().CombineStack(entity);

                // Return whatever is left over from the merge
                return entity;
            }

            // Put entity in storage, replacing whatever was there
            Storage[slot] = entity;
            Owner.Hierarchy.AddChild(entity);

            return current;
        }

        /// <summary>
        /// Accesses an entity in a container slot.
        /// </summary>
        /// <param name="slot">The slot to access</param>
        /// <returns>The entity in the slot (may be null)</returns>
        public Entity Peek(int slot)
        {
            return Storage[slot];
        }

        /// <summary>
        /// Takes an entity from a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The entity taken (may be null)</returns>
        public Entity Take(int slot)
        {
            Entity current = Storage[slot];
            Storage[slot] = null;
            return current;
        }

        /// <summary>
        /// Takes a single entity from a stack in a container slot.
        /// </summary>
        /// <param name="slot">The slot to take from</param>
        /// <returns>The entity taken (may be null)</returns>
        public Entity TakeSingle(int slot)
        {
            Entity current = Storage[slot];
            if (current != null)
            {
                var stack = current.GetModule<StackingModule>();
                if (stack != null)
                {
                    if (stack.Amount > 1)
                    {
                        return stack.TakeFromStack(1);
                    }
                }
            }

            return Take(slot);
        }

        public Entity TakeHalf(int slot)
        {
            Entity current = Storage[slot];
            if (current != null)
            {
                var stack = current.GetModule<StackingModule>();
                if (stack != null)
                {
                    if (stack.Amount > 1)
                    {
                        return stack.TakeFromStack(stack.Amount / 2);
                    }
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
                    Storage[i].Hierarchy.MakeIndependent(dropPosition);
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