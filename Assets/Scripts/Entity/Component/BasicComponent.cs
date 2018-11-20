using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Entity.Type;

namespace Entity.Component
{
    public abstract class BasicComponent : ScriptableObject
    {
        public static T LoadTemplate<T>(T template) where T : BasicComponent
        {
            if (template == null)
            {
                // Default values
                return CreateInstance<T>();
            }

            return template.Clone() as T;
        }

        public BasicEntity Owner;

        public void Initialize(BasicEntity entity)
        {
            Owner = entity;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {

        }

        protected abstract BasicComponent Clone();
    }
}
