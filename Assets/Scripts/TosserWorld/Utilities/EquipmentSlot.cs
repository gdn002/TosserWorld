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

        public void Remove()
        {
            Equipped = null;
        }

        public void DropEquipped()
        {
            Entity drop = Equipped;
            drop.SetParent(null);
            drop.Position = drop.Position + new Vector2(0.5f, 0);
            drop.transform.localScale = Vector3.one;
        }
    }
}
