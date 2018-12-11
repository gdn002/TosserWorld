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
                entity.SetRootMode(false);
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
            Equipped.Position = Equipped.Position + Vector2.right;
            Equipped.transform.localScale = Vector3.one;
            Equipped.SetRootMode(true);
            Equipped = null;
        }
    }
}
