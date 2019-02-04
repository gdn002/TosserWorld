using TosserWorld;
using UnityEngine;

namespace Utility
{
    public class EquipmentSlot : MonoBehaviour
    {
        public Entity Equipped { get; private set; }

        public void AddToSlot(Entity entity)
        {
            if (Equipped != null)
                DropEquipped();

            if (entity != null)
            {
                entity.SetAsChild(true);
                entity.transform.SetParent(transform);
                entity.transform.localPosition = Vector2.zero;
                entity.transform.localScale = Vector3.one;
                entity.transform.localRotation = Quaternion.identity;

                Equipped = entity;
            }
        }

        public void DropEquipped()
        {
            Equipped.transform.SetParent(null);
            Equipped.Position = Equipped.Position + new Vector2(0.5f, 0);
            Equipped.transform.localScale = Vector3.one;
            Equipped.SetAsChild(false);
            Equipped = null;
        }
    }
}
