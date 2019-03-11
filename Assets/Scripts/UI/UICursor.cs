using UnityEngine;
using UnityEngine.UI;

using TosserWorld.Entities;
using TosserWorld.Modules;

namespace TosserWorld.UI
{
    class UICursor : MonoBehaviour
    {
        public static UICursor Cursor { get; private set; }


        public Entity AttachedEntity { get; private set; }
        private GameObject FloatingIcon;

        void Awake()
        {
            Cursor = this;
        }

        void Update()
        {
            if (AttachedEntity != null)
            {
                if (!AttachedEntity.Hierarchy.IsChildOf(PlayerEntity.Player))
                {
                    AttachedEntity = null;
                    DestroyIcon();
                }
            }

            if (FloatingIcon != null)
            {
                FloatingIcon.transform.position = Input.mousePosition;
            }
        }

        public void SetAttachedEntity(Entity entity = null)
        {
            AttachedEntity = entity;
            if (AttachedEntity == null)
            {
                DestroyIcon();
            }
            else
            {
                PlayerEntity.Player.Hierarchy.AddChild(AttachedEntity);
                CreateIcon();
            }
        }

        public void SetAttachedEntity(Module module)
        {
            if (module == null)
                SetAttachedEntity();
            else
                SetAttachedEntity(module.Owner);
        }

        public void DropAttachedEntity(Vector2 worldPosition)
        {
            AttachedEntity.Hierarchy.MakeIndependent(worldPosition, true);
            SetAttachedEntity();
        }

        private void CreateIcon()
        {
            DestroyIcon();

            // TODO: Maybe have a prefab for this
            FloatingIcon = new GameObject();
            var image = FloatingIcon.AddComponent<Image>();
            image.sprite = AttachedEntity.InventorySprite;
            image.raycastTarget = false;
            
            FloatingIcon.transform.SetParent(transform);
        }

        private void DestroyIcon()
        {
            if (FloatingIcon != null)
            {
                Destroy(FloatingIcon);
                FloatingIcon = null;
            }
        }
    }
}
