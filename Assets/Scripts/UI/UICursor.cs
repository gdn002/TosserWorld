using UnityEngine;
using UnityEngine.UI;

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
            if (FloatingIcon != null)
            {
                FloatingIcon.transform.position = Input.mousePosition;
            }
        }

        public void SetAttachedEntity(Entity entity)
        {
            AttachedEntity = entity;
            if (AttachedEntity == null)
            {
                DestroyIcon();
            }
            else
            {
                CreateIcon();
            }
        }

        public void DropAttachedEntity(Vector2 worldPosition)
        {
            AttachedEntity.SetAsSubEntity(false);
            AttachedEntity.EnableRendering();
            AttachedEntity.transform.position = worldPosition;
            AttachedEntity.transform.SetParent(null);

            SetAttachedEntity(null);
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
