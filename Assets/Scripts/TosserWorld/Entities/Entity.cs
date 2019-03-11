using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using TosserWorld.Modules;
using TosserWorld.Modules.Configurations;
using TosserWorld.Utilities;
using Utility.Enumerations;

namespace TosserWorld.Entities
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    [System.Serializable]
    public class Entity : MonoBehaviour
    {
        // TODO: Move this somewhere better
        public static EntityChunk GlobalChunk = new EntityChunk();

        public bool IsChild { get; protected set; }

        // Used internally to ensure all components have been initialized
        private bool IsInitialized = false;

        public Entity Clone()
        {
            Entity clone = Instantiate(gameObject, transform.parent).GetComponent<Entity>();
            clone.name = name;
            clone.Start();
            return clone;
        }

        // ---- STATS ----

        // The entity's name
        public string Name = "Generic Object";

        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }

        public Sprite InventorySprite;


        // ---- MODULES ----

        // Module Enablers
        public bool EnableBrain;
        public bool EnableInventory;
        public bool EnableInteraction;
        public bool EnableMovement;
        public bool EnablePhysics;
        public bool EnableStacking;
        public bool EnableStats;

        // Module Configurations
        public BrainConfig          BrainConfig;
        public InventoryConfig      InventoryConfig;
        public InteractionConfig    InteractionConfig;
        public MovementConfig       MovementConfig;
        public PhysicsConfig        PhysicsConfig;
        public StackingConfig       StackingConfig;
        public StatsConfig          StatsConfig;
        public TagListConfig        TagListConfig;

        // Modules
        public BrainModule          Brain;
        public InventoryModule      Inventory;
        public InteractionModule    Interaction;
        public MovementModule       Movement;
        public PhysicsModule        Physics;
        public StackingModule       Stacking;
        public StatsModule          Stats;
        public TagListModule        TagList;

        // Module List for iterating
        public List<Module> ModuleList = new List<Module>();

        private void LoadModules()
        {
            Brain       = LoadModule<BrainModule        >(EnableBrain       , BrainConfig       );
            Inventory   = LoadModule<InventoryModule    >(EnableInventory   , InventoryConfig   );
            Interaction = LoadModule<InteractionModule  >(EnableInteraction , InteractionConfig );
            Movement    = LoadModule<MovementModule     >(EnableMovement    , MovementConfig    );
            Physics     = LoadModule<PhysicsModule      >(EnablePhysics     , PhysicsConfig     );
            Stacking    = LoadModule<StackingModule     >(EnableStacking    , StackingConfig    );
            Stats       = LoadModule<StatsModule        >(EnableStats       , StatsConfig       );
            TagList     = LoadModule<TagListModule      >(true              , TagListConfig     );

        }

        private M LoadModule<M>(bool enable, ModuleConfiguration configuration) where M : Module, new()
        {
            if (enable)
            {
                M module = new M();
                ModuleList.Add(module);
                module.Initialize(this, configuration);
                return module;
            }

            return null;
        }


        // ---- UTILITIES ----

        protected virtual void LoadControllers()
        {
            // Load all utilities here

            OrientationController.Load(this);
            EquipmentSlots.Load(this);

            Render = GetComponentInChildren<EntityRenderer>();
            FlippableSprite = GetComponentInChildren<FlippableSprite>();

            Hierarchy = new EntityHierarchy(this);
        }


        // ---- RENDERING ----

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

        protected EntityOrientation OrientationController = new EntityOrientation();
        public Orientation Orientation { get { return OrientationController.LocalOrientation; } set { OrientationController.LocalOrientation = value; } }
 

        // ---- FLIPPING ----

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

        public EntityEquipment EquipmentSlots = new EntityEquipment();


        // ---- HIERARCHY ----

        public EntityHierarchy Hierarchy;


        // ---- REFERENCES ----

        // Unity references
        public Rigidbody2D RigidBody;
        public Animator Animator;
        public Collider2D MainCollider;

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
                LoadModules();

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
                foreach (var module in ModuleList)
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
                foreach (var module in ModuleList)
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
                if (Interaction != null)
                {
                    PlayerEntity.Player.QueueInteraction(this);
                }
            }
        }


        // ---- UTILITY FUNCTIONS ----

        public void Destroy()
        {
            Destroy(gameObject);
        }

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

        

    }
}
