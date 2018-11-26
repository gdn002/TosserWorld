using UnityEngine;

namespace TosserWorld.Modules
{
    public abstract class Module : ScriptableObject
    {
        public static Module LoadTemplate(Module template)
        {
            return template.Clone();
        }

        public Entity Owner;


        public virtual void Update()
        {
            // This function will be called in the Owner's Update() function
        }

        public virtual void LateUpdate()
        {
            // This function will be called in the Owner's LateUpdate() function
        }

        public void Initialize(Entity entity)
        {
            Owner = entity;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            // This function will be called in the Owner's Start() function
        }

        protected abstract Module Clone();
    }
}
