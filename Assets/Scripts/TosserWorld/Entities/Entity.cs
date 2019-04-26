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
        // Used internally to ensure all components have been initialized
        private bool IsInitialized = false;

        public bool IsAlive { get; protected set; }

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
        public bool EnableAction;
        public bool EnableBrain;
        public bool EnableInventory;
        public bool EnableInteraction;
        public bool EnableMovement;
        public bool EnablePhysics;
        public bool EnableStacking;
        public bool EnableStats;

        // Module Configurations
        public ActionConfig         ActionConfig;
        public BrainConfig          BrainConfig;
        public InventoryConfig      InventoryConfig;
        public InteractionConfig    InteractionConfig;
        public MovementConfig       MovementConfig;
        public PhysicsConfig        PhysicsConfig;
        public StackingConfig       StackingConfig;
        public StatsConfig          StatsConfig;
        public TagListConfig        TagListConfig;

        // Modules
        public ActionModule         Action;
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
            Action      = LoadModule<ActionModule       >(EnableAction      , ActionConfig      );
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
        /// Shows or hides the entity.
        /// </summary>
        /// <param name="hide">True to disable entity rendering, false to enable it.</param>
        public void Hide(bool hide = true)
        {
            Render.EnableRendering(!hide);
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

        public Entity Parent { get; protected set; }
        public bool IsChild { get { return Parent != null; } }



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

                EntityChunk.GlobalChunk.AddEntity(this);

                // Object is ready for action
                IsInitialized = true;
                IsAlive = true;
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
            EntityChunk.GlobalChunk.RemoveEntity(this);

            if (Parent != null)
            {
                Parent.Inventory.Remove(Stacking);
            }
        }


        // ---- EVENT HANDLERS ----

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (SpriteRenderer renderer in Render.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.white;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (SpriteRenderer renderer in Render.GetComponentsInChildren<SpriteRenderer>())
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
            else
            {
                if (Stats != null)
                {
                    Stats.Health.Modify(-25);
                }
            }
        }


        // ---- UTILITY FUNCTIONS ----

        public void ActivateEquipment(bool hold = false, int equipmentSlot = 0)
        {
            if (EquipmentSlots.Length > equipmentSlot)
            {
                var equipped = EquipmentSlots[equipmentSlot].Equipped;
                if (equipped != null && equipped.Action != null)
                {
                    equipped.Action.Activate(this, hold);
                    return;
                }
            }

            ActivateDefaultAction(hold);
        }

        public void ActivateDefaultAction(bool hold = false)
        {
            if (Action != null)
            {
                Action.Activate(this, hold);
            }
        }

        public void ActivateAction(Entity actor, bool hold = false)
        {
            if (Action != null)
            {
                Action.Activate(actor, hold);
            }
        }

        public void Kill()
        {
            if (IsAlive)
            {
                IsAlive = false;
                if (Movement != null)
                    Movement.Stop();

                // If we have an animator, play the "Die" animation
                // DeathAnimationBehaviour will ensure Destroy() gets called when the death animation ends
                if (Animator != null)
                {
                    Animator.SetTrigger("Die");
                }
                // Otherwise just call Destroy()
                else
                {
                    Remove(true);
                }
            }
        }

        public void Remove(bool usePoofEffect = false)
        {
            if (usePoofEffect)
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/smokePuff"), Position, CameraController.CameraRotation);
            }

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

        /// <summary>
        /// Sets a new parent entity for this entity. The entity will be effectively "attached" to its parent until it is made independent again.
        /// Setting a new parent for this entity will remove it from the inventory or equipment slot it is currently in (if it applies).
        /// </summary>
        /// <param name="parent">The new parent entity. Can be null in order to make this entity independent.</param>
        public void SetParent(Entity parent)
        {
            if (Parent == parent)
                return; // Entity already has this parent

            if (IsChild)
            {
                // If the entity had a different parent, clean it up from its parent's equipment/inventory
                RemoveFromParent();
            }

            // Set the new parent
            Parent = parent;
            if (IsChild)
            {
                // The entity was given a new parent
                transform.SetParent(Parent.transform);
                transform.localPosition = Vector3.zero;

                if (RigidBody != null) RigidBody.isKinematic = true;
                if (MainCollider != null) MainCollider.enabled = false;

                EnableIsometry(false);
                Render.EnableSelection(false);
            }
            else
            {
                // The entity was made independent

                transform.localPosition = Vector2.zero;
                transform.SetParent(null, true);

                if (RigidBody != null) RigidBody.isKinematic = false;
                if (MainCollider != null) MainCollider.enabled = true;

                EnableIsometry(true);
                Render.EnableSelection(true);
            }
        }

        private void RemoveFromParent()
        {
            if (IsChild)
            {
                if (Parent.EquipmentSlots.Remove(this))
                    return;

                if (Parent.Inventory != null)
                {
                    Parent.Inventory.Remove(Stacking);
                }
            }
        }
    }
}
