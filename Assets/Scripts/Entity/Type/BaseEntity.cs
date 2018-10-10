using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Entity.Component;

namespace Entity.Type
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public class BaseEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // TODO: Move this somewhere better
        public static EntityChunk GlobalChunk = new EntityChunk();

        // Name
        public string EntityName;

        // Tags
        public List<EntityTags> Tags = new List<EntityTags>();
        public EntityInteractions DefaultInteraction = EntityInteractions.None;

        // Sprites
        public Sprite InventorySprite;

        // References for Unity components
        public Rigidbody2D RigidBody { get; private set; }
        public Animator Animator { get; private set; }

        // References for Entity components
        public PhysicsComponent Physics { get; private set; }
        public ContainerComponent Container { get; private set; }

        public GameObject Render { get; private set; }

        // Use this for initialization
        void Start()
        {
            GlobalChunk.AddEntity(this);

            // Detect and fill Unity component references
            RigidBody = GetComponent<Rigidbody2D>();
            Animator = GetComponentInChildren<Animator>();

            // Detect and fill Entity component references
            Physics = GetComponent<PhysicsComponent>();
            Container = GetComponent<ContainerComponent>();

            Render = transform.Find("Render").gameObject;

            OnStart();
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdate();
        }

        void OnDestroy()
        {
            GlobalChunk.RemoveEntity(this);
        }


        //// ---- ENTITY BEHAVIOUR ----

        /// <summary>
        /// Implement entity initial setup
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Implement entity behaviour
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// Implement entity behaviour in containers
        /// </summary>
        public virtual void InContainerUpdate()
        {
        }


        //// ---- INTERACTION FUNCTIONS ----

        /// <summary>
        /// Performs an interaction between this entity and another.
        /// </summary>
        /// <param name="entity">The entity that triggered the interaction</param>
        /// <param name="interaction">The type of interaction triggered</param>
        public void DoInteraction(BaseEntity entity, EntityInteractions interaction)
        {
            
        }

        /// <summary>
        /// Performs the default interaction between this entity and another
        /// </summary>
        /// <param name="entity">The entity that triggered the interaction</param>
        public void DoDefaultInteraction(BaseEntity entity)
        {
            DoInteraction(entity, DefaultInteraction);
        }

        /// <summary>
        /// Performs an interaction between this entity and the player.
        /// </summary>
        /// <param name="interaction">The type of interaction triggered</param>
        public void DoPlayerInteraction(EntityInteractions interaction)
        {
            DoInteraction(PlayerEntity.Player, interaction);
        }

        /// <summary>
        /// Performs the default interaction between this entity and the player
        /// </summary>
        public void DoDefaultPlayerInteraction()
        {
            DoPlayerInteraction(DefaultInteraction);
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
                DoDefaultPlayerInteraction();
            }
        }

        public virtual void OnAddedToContainer(ContainerComponent container)
        {
            SetEnable(false);
            transform.SetParent(container.transform, false);
            transform.localPosition = Vector3.zero;
        }

        public virtual void OnRemovedFromContainer(ContainerComponent container)
        {
            SetEnable(true);
            transform.localPosition = Vector3.right;
            transform.SetParent(null);
        }


        //// ---- UTILITY FUNCTIONS ----

        public void SetEnable(bool enable)
        {
            Render.SetActive(enable);
            RigidBody.isKinematic = !enable;
        }

        public bool HasTag(EntityTags tag)
        {
            if (tag == EntityTags.Any)
                return true;

            return Tags.Contains(tag);
        }

        public float DistanceTo(BaseEntity entity)
        {
            return Vector2.Distance(transform.position, entity.transform.position);
        }
    }
}
