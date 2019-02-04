using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using TosserWorld.Modules;
using Utility;
using Utility.Enumerations;

namespace TosserWorld
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public class Entity : MonoBehaviour
    {
        // TODO: Move this somewhere better
        public static EntityChunk GlobalChunk = new EntityChunk();

        // The entity's name
        public string Name = "Generic Object";

        public bool IsChild { get; protected set; }

        // Optional modules
        public List<Module> Modules = new List<Module>();
        
        // Used internally to ensure all components have been initialized
        private bool IsInitialized = false;



        public Entity Clone()
        {
            Entity clone = Instantiate(gameObject, transform.parent).GetComponent<Entity>();
            clone.name = name;
            clone.Start();
            return clone;
        }

        // Load all utilities here
        private void LoadControllers()
        {
            OrientationController.Load(this);
            EquipmentSlots.Load(this);

            Render = GetComponentInChildren<EntityRenderer>();
            FlippableSprite = GetComponentInChildren<FlippableSprite>();

            Hierarchy = new EntityHierarchy(this);
        }


        // ---- RENDERING ----
        // Utility for ensuring sprites always face the camera and are sorted by world position accordingly

        protected EntityRenderer Render { get; private set; }

        public void EnableIsometry(bool enable = true)
        {
            if (Render != null)
            {
                Render.ResetRotation();
                Render.EnableIsometry = enable;
            }
        }

        /// <summary>
        /// Enables or disables rendering features, effectively showing or hiding the entity.
        /// </summary>
        /// <param name="enable">Enables or disables rendering features for the entity</param>
        public void EnableRendering(bool enable = true)
        {
            Render.EnableRendering(enable);
        }
        

        // ---- ORIENTATION ----
        // Controller for keeping orientation data and managing orientation-controlled sprites

        protected class EntityOrientation
        {
            public Orientation LocalOrientation = Orientation.N;

            private Orientation? LastCamera = null;
            private Orientation? LastLocal = null;

            private OrientationControlledSprite[] Sprites;

            public void Load(Entity owner)
            {
                Sprites = owner.GetComponentsInChildren<OrientationControlledSprite>();
            }

            public void UpdateSprites()
            {
                if (LastCamera == null || LastCamera != CameraController.Controller.Orientation.CurrentOrientation || LastLocal != LocalOrientation)
                {
                    LastCamera = CameraController.Controller.Orientation.CurrentOrientation;
                    LastLocal = LocalOrientation;

                    foreach (var sprite in Sprites)
                    {
                        sprite.UpdateOrientation(LocalOrientation, LastCamera.Value);
                    }
                }
            }

            public int SpriteCount()
            {
                return Sprites.Length;
            }
        }

        protected EntityOrientation OrientationController = new EntityOrientation();

        public Orientation Orientation { get { return OrientationController.LocalOrientation; } set { OrientationController.LocalOrientation = value; } }
 

        // ---- FLIPPING ----
        // Utility for flipping sprites around

        protected FlippableSprite FlippableSprite;

        public void FlipTo(Vector2 direction)
        {
            if (FlippableSprite != null)
                FlippableSprite.FlipTo(direction);
        }

        public void FlipToScreen(Vector2 direction)
        {
            if (FlippableSprite != null)
                FlippableSprite.FlipToScreen(direction);
        }

        public void FlipTo(bool direction)
        {
            if (FlippableSprite != null)
                FlippableSprite.FlipTo(direction);
        }

        // ---- EQUIPMENT SLOTS ----
        // Utility for "equipping" other entities

        public class EquipSlotsController
        {
            private EquipmentSlot[] Slots;

            public void Load(Entity owner)
            {
                Slots = owner.GetComponentsInChildren<EquipmentSlot>();
            }

            public EquipmentSlot this[int i]
            {
                get { return Slots[i]; }
            }

            public void DropAll()
            {
                foreach (var slot in Slots)
                {
                    slot.DropEquipped();
                }
            }

            public int Length { get { return Slots.Length; } }
        }

        public EquipSlotsController EquipmentSlots = new EquipSlotsController();

        // ---- HIERARCHY ----
        // Utility for keeping track of entity hierarchy

        public class EntityHierarchy : IEnumerable<Entity>
        {
            protected Entity Self;
            protected Entity Parent;
            protected List<Entity> Children = new List<Entity>();

            public EntityHierarchy(Entity self)
            {
                Self = self;
            }

            public void AddChild(Entity entity, bool disableRendering = true)
            {
                if (entity.Hierarchy.Parent == Self)
                    return; // Entity is already childed

                entity.Hierarchy.MakeIndependent();
                
                entity.SetAsChild(true);
                entity.EnableRendering(!disableRendering);

                entity.transform.SetParent(Self.transform);
                entity.transform.localPosition = Vector3.zero;

                Children.Add(entity);
                entity.Hierarchy.Parent = Self;
            }

            public IEnumerator<Entity> GetEnumerator()
            {
                foreach (var child in Children)
                {
                    yield return child;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void MakeIndependent()
            {
                MakeIndependent(Vector2.zero);
            }

            public void MakeIndependent(Vector2 dropPosition, bool setWorldPosition = false)
            {
                if (Parent != null)
                {
                    Self.SetAsChild(false);
                    Self.EnableRendering();

                    Parent.Hierarchy.Children.Remove(Self);
                    Parent = null;

                    if (setWorldPosition) Self.transform.position = dropPosition;
                    else                  Self.transform.localPosition = dropPosition;

                    Self.transform.SetParent(null, true);
                }
            }
        }

        public EntityHierarchy Hierarchy;

        // ---- STATS ----

        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }


        // ---- REFERENCES ----

        // Unity references
        public Rigidbody2D RigidBody;
        public Animator Animator;
        public Collider2D MainCollider;

        // Sprites
        public Sprite InventorySprite;

        // Use this for initialization
        protected virtual void Start()
        {
            if (!IsInitialized)
            {
                // Load Unity references
                RigidBody       = GetComponent<Rigidbody2D>();
                Animator        = GetComponentInChildren<Animator>();
                MainCollider    = GetComponent<Collider2D>();

                LoadControllers();

                // Load template modules as their own object
                for (int i = 0; i < Modules.Count; i++)
                {
                    Modules[i] = Module.LoadTemplate(Modules[i]);
                    Modules[i].Initialize(this);
                }

                GlobalChunk.AddEntity(this);

                // Object is ready for action
                IsInitialized = true;
                IsChild = false;
            }

            if (InventorySprite == null)
            {
                InventorySprite = GetComponentInChildren<SpriteRenderer>().sprite; // TODO: Temporary hack to auto generate inventory sprites
            }
        }

        protected virtual void Update()
        {
            if (IsInitialized)
            {
                foreach (var module in Modules)
                {
                    module.Update();
                }

                OrientationController.UpdateSprites();
            }
        }

        protected virtual void LateUpdate()
        {
            if (IsInitialized)
            {
                foreach (var module in Modules)
                {
                    module.LateUpdate();
                }
            }
        }

        void OnDestroy()
        {
            GlobalChunk.RemoveEntity(this);
        }


        // ---- EVENT HANDLERS ----

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.white;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.yellow;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (GetModule<InteractionModule>() != null)
                {
                    PlayerEntity.Player.QueueInteraction(this);
                }
            }
        }


        // ---- UTILITY FUNCTIONS ----

        /// <summary>
        /// Operates the same way as GetComponentsInChildren, except it accounts for sub-entities.
        /// </summary>
        /// <typeparam name="T">The component type to search</typeparam>
        /// <returns>A list of all the components owned by this entity</returns>
        public List<T> GetComponentsInEntity<T>() where T : Component
        {
            List<T> components = new List<T>();

            // Add components in the entity itself
            components.AddRange(GetComponents<T>());

            // Iterate through the entity's child objects
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;

                // Only add the child's components if it isn't its own entity
                if (child.GetComponent<Entity>() == null)
                {
                    // Search recursively
                    components.AddRange(InternalGetComponentsInEntity<T>(child));
                }
            }

            return components;
        }

        private List<T> InternalGetComponentsInEntity<T>(GameObject obj) where T : Component
        {
            List<T> components = new List<T>();
            components.AddRange(obj.GetComponents<T>());

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i).gameObject;
                if (child.GetComponent<Entity>() == null)
                {
                    components.AddRange(InternalGetComponentsInEntity<T>(child));
                }
            }

            return components;
        }

        /// <summary>
        /// Finds and return a module attached to this entity.
        /// </summary>
        /// <typeparam name="T">The module type to search</typeparam>
        /// <returns>A reference to the module, or null if it couldn't be found in the entity</returns>
        public T GetModule<T>() where T : Module
        {
            foreach (var module in Modules)
            {
                if (typeof(T) == module.GetType())
                    return module as T;
            }

            return null;
        }
        
        public bool HasTag(EntityTags tag)
        {
            if (tag == EntityTags.Any)
                return true;

            return GetModule<TagListModule>().Tags.Contains(tag);
        }

        public float DistanceTo(Entity entity)
        {
            return Vector2.Distance(transform.position, entity.transform.position);
        }

        public void SetAsChild(bool child)
        {
            IsChild = child;
            if (RigidBody != null) RigidBody.isKinematic = child;
            if (MainCollider != null) MainCollider.enabled = !child;

            EnableIsometry(!child);
            OnPointerExit(null);
        }

        public bool MatchStacks(Entity other)
        {
            if (other == null)
                return false;

            // To match stacks, the names of the entities must match...
            if (Name != other.Name)
                return false;

            // And both entities must be stackable (contain stack components)
            return (GetModule<StackingModule>() != null && other.GetModule<StackingModule>() != null);
        }

    }
}
