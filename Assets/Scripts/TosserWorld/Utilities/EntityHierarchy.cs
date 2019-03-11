using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TosserWorld.Modules;
using TosserWorld.Entities;

namespace TosserWorld.Utilities
{
    // Utility for keeping track of entity hierarchy
    public class EntityHierarchy : IEnumerable<Entity>
    {
        protected Entity Self;
        protected Entity Parent;
        protected List<Entity> Children = new List<Entity>();

        public EntityHierarchy(Entity self)
        {
            Self = self;
        }

        public bool IsChildOf(Entity entity)
        {
            return Parent == entity;
        }

        public bool IsChildOf(Module module)
        {
            return Parent == module.Owner;
        }

        public void AddChild(Entity entity, bool disableRendering = true)
        {
            if (entity.Hierarchy.Parent == Self)
                return; // Entity is already childed

            entity.Hierarchy.MakeIndependent();

            entity.SetAsChild(true);
            entity.EnableRendering(!disableRendering);

            entity.transform.SetParent(Self.transform);
            entity.transform.localPosition = Vector3.zero;

            Children.Add(entity);
            entity.Hierarchy.Parent = Self;
        }

        public void AddChild(Module module, bool disableRendering = true)
        {
            AddChild(module.Owner, disableRendering);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            foreach (var child in Children)
            {
                yield return child;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void MakeIndependent()
        {
            MakeIndependent(Vector2.zero);
        }

        public void MakeIndependent(Vector2 dropPosition, bool setWorldPosition = false)
        {
            if (Parent != null)
            {
                Self.SetAsChild(false);
                Self.EnableRendering();

                Parent.Hierarchy.Children.Remove(Self);
                Parent = null;

                if (setWorldPosition) Self.transform.position = dropPosition;
                else Self.transform.localPosition = dropPosition;

                Self.transform.SetParent(null, true);
            }
        }
    }
}
