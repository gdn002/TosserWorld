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
    public class Entity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // TODO: Move this somewhere better
        public static EntityChunk GlobalChunk = new EntityChunk();

        // The entity's name
        public string Name = "GENERIC_ENTITY";

        public bool RootMode { get; protected set; }

        // Optional modules
        public List<Module> Modules = new List<Module>();
        
        // Used internally to ensure all components have been initialized
        private bool IsInitialized = false;



        // Load all utilities here
        private void LoadControllers()
        {
            OrientationController.Load(this);
            EquipmentSlots.Load(this);

            IsometricSprite = GetComponentInChildren<IsometricSprite>();
            FlippableSprite = GetComponentInChildren<FlippableSprite>();

        }


        // ---- ISOMETRIC SPRITE ----
        // Utility for ensuring sprites always face the camera and are sorted by world position accordingly

        protected IsometricSprite IsometricSprite { get; private set; }

        public void EnableIsometricSorting(bool enable = true)
        {
            if (IsometricSprite != null)
            {
                IsometricSprite.ResetRotation();
                IsometricSprite.Enabled = enable;
            }
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
        }

        public EquipSlotsController EquipmentSlots = new EquipSlotsController();

        // ---- STATS ----

        public Vector2 Position { get { return transform.position; } set { transform.position = value; } }


        // ---- REFERENCES ----

        // Unity references
        public Rigidbody2D RigidBody;
        public Animator Animator;
        public Collider2D MainCollider;

        // Hierarchy references
        public GameObject Render;

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

                // Load hierarchy references
                Render = transform.Find("Render").gameObject;

                // Load template modules as their own object
                for (int i = 0; i < Modules.Count; i++)
                {
                    Modules[i] = Module.LoadTemplate(Modules[i]);
                    Modules[i].Initialize(this);
                }

                LoadControllers();
                GlobalChunk.AddEntity(this);

                // Object is ready for action
                IsInitialized = true;
                RootMode = true;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.yellow;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.white;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // TODO
            }
        }

        public virtual void OnAddedToContainer(ContainerModule inventory)
        {
            SetRootMode(false);
            transform.SetParent(inventory.Owner.transform, false);
            transform.localPosition = Vector3.zero;
        }

        public virtual void OnRemovedFromContainer(ContainerModule inventory)
        {
            SetRootMode(true);
            transform.localPosition = Vector3.right;
            transform.SetParent(null);
        }


        // ---- UTILITY FUNCTIONS ----

        public T GetModule<T>() where T : Module
        {
            foreach (var module in Modules)
            {
                if (typeof(T) == module.GetType())
                    return module as T;
            }

            return null;
        }

        /// <summary>
        /// Use this function to enable or disable root features for this entity.
        /// </summary>
        /// <param name="enable">True enables root features for the entity, while false disables them</param>
        public void SetRootMode(bool enable = true)
        {
            RootMode = enable;
            //Render.SetActive(enable);
            if (RigidBody != null) RigidBody.isKinematic = !enable;
            if (MainCollider != null) MainCollider.enabled = enable;

            EnableIsometricSorting(enable);
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

        public bool MatchStacks(Entity other)
        {
            // To match stacks, the names of the entities must match...
            if (Name != other.Name)
            {
                return false;
            }

            // And both entities must be stackable (contain stack components)
            return (GetModule<StackingModule>() != null && other.GetModule<StackingModule>() != null);
        }

    }
}
