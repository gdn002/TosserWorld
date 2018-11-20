using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Entity.Component;
using Entity.Util;

namespace Entity.Type
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public class BasicEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // TODO: Move this somewhere better
        public static EntityChunk GlobalChunk = new EntityChunk();


        public BasicData Data;
        public BasicInventory Inventory;
        public BasicPhysics Physics;
        public BasicStack Stack;

        protected bool IsInitialized = false;

        //// ---- ENTITY STATS ----

        public Stat Radiation;

        public Vector2 Position { get { return transform.position; } }

        // References for components
        public struct EntityComponents
        {
            // Unity components
            public Rigidbody2D RigidBody;
            public Animator Animator;
            public Collider2D MainCollider;

            // Hierarchy
            public GameObject Render;
        }
        public EntityComponents Components;

        // Sprites
        public Sprite InventorySprite;

        // Use this for initialization
        protected virtual void Start()
        {
            // Detect and fill Unity component references
            Components.RigidBody = GetComponent<Rigidbody2D>();
            Components.Animator = GetComponentInChildren<Animator>();
            Components.MainCollider = GetComponent<Collider2D>();

            Components.Render = transform.Find("Render").gameObject;

            // Load components into their own instances
            Data       = BasicComponent.LoadTemplate(Data);
            Inventory  = BasicComponent.LoadTemplate(Inventory);
            Physics    = BasicComponent.LoadTemplate(Physics);
            Stack      = BasicComponent.LoadTemplate(Stack);

            // Initialize entity components
            Data.Initialize(this);
            Inventory.Initialize(this);
            Physics.Initialize(this);
            Data.Initialize(this);

            IsInitialized = true;

            GlobalChunk.AddEntity(this);
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void LateUpdate()
        {
            if (IsInitialized)
            {
                if (Physics != null) Physics.SimulatePhysics();
            }
        }

        void OnDestroy()
        {
            GlobalChunk.RemoveEntity(this);
        }


        //// ---- EVENT HANDLERS ----

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

        public virtual void OnAddedToContainer(BasicInventory inventory)
        {
            SetEnable(false);
            transform.SetParent(inventory.Owner.transform, false);
            transform.localPosition = Vector3.zero;
        }

        public virtual void OnRemovedFromContainer(BasicInventory inventory)
        {
            SetEnable(true);
            transform.localPosition = Vector3.right;
            transform.SetParent(null);
        }


        //// ---- UTILITY FUNCTIONS ----

        public void SetEnable(bool enable)
        {
            Components.Render.SetActive(enable);
            Components.RigidBody.isKinematic = !enable;
        }

        public bool HasTag(EntityTags tag)
        {
            if (tag == EntityTags.Any)
                return true;

            return Data.Tags.Contains(tag);
        }

        public float DistanceTo(BasicEntity entity)
        {
            return Vector2.Distance(transform.position, entity.transform.position);
        }

        public bool MatchStacks(BasicEntity other)
        {
            // To match stacks, the names of the entities must match...
            if (Data.Name != other.Data.Name)
            {
                return false;
            }

            // And both entities must be stackable (contain stack components)
            return (Stack != null && other.Stack != null);
        }
    }
}
