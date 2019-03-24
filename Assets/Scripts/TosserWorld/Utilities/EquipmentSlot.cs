using UnityEngine;

using TosserWorld.Entities;

namespace TosserWorld.Utilities
{
    public class EquipmentSlot : MonoBehaviour
    {
        public Entity Owner { get; set; }
        public Entity Equipped { get; private set; }

        public void AddToSlot(Entity entity)
        {
            if (Equipped != null)
                DropEquipped();

            if (entity != null)
            {
                entity.SetParent(Owner);
                entity.transform.SetParent(transform);

                entity.transform.localPosition = Vector3.zero;
                entity.transform.localScale = Vector3.one;
                entity.transform.localRotation = Quaternion.identity;

                Equipped = entity;
            }
        }

        public void DropEquipped()
        {
            Equipped.SetParent(null);
            Equipped.Position = Equipped.Position + new Vector2(0.5f, 0);
            Equipped.transform.localScale = Vector3.one;
            Equipped = null;
        }
    }
}
