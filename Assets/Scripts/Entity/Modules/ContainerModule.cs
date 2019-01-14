using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Container Module", menuName = "Modules/Container")]
    public class ContainerModule : Module
    {
        private static GameObject UIPrefab;
        private static Canvas UICanvas;

        public InventorySpace Storage;

        public int Rows;
        public int Cols;

        public int SlotCount { get { return Rows * Cols; } }

        private GameObject InventoryPanel;


        public ContainerModule()
        {
            Rows = 3;
            Cols = 3;
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
            UIPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/InventoryPanel");
            UICanvas = GameObject.FindObjectOfType<Canvas>();

            Storage = new InventorySpace(SlotCount);
        }

        public void UpdateContents()
        {
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] != null)
                {
                    // TODO
                    //Storage[i].InContainerUpdate();
                }
            }
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
                if (Storage[i] != null)
                {
                    if (Storage[i].MatchStacks(entity))
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
            }

            // If there's still leftover items, place them in the first empty slot
            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] == null)
                {
                    // Put this entity in storage
                    Storage[i] = entity;
                    Pack(entity);
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
            if (current != null)
            {
                if (current.MatchStacks(entity))
                {
                    // Merge stacks
                    entity = Storage[slot].GetModule<StackingModule>().CombineStack(entity);

                    // Return whatever is left over from the merge
                    return entity;
                }
            }

            // Put entity in storage, replacing whatever was there
            Storage[slot] = entity;
            Pack(entity);

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

        public void DropAll()
        {
            Vector2 dropPosition = Vector2.right;

            for (int i = 0; i < Storage.Length; ++i)
            {
                if (Storage[i] != null)
                {
                    Unpack(Storage[i], dropPosition);
                    Storage[i] = null;

                    dropPosition = Quaternion.Euler(0, 0, -15) * dropPosition;
                }
            }
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


        private void Pack(Entity entity)
        {
            entity.SetAsSubEntity();
            entity.EnableRendering(false);
            entity.transform.SetParent(Owner.transform, false);
            entity.transform.localPosition = Vector3.zero;
        }

        private void Unpack(Entity entity, Vector2 dropAt)
        {
            entity.SetAsSubEntity(false);
            entity.EnableRendering();
            entity.transform.localPosition = dropAt;
            entity.transform.SetParent(null);
        }
    }
}