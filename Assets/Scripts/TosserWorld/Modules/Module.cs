using TosserWorld.Entities;
using TosserWorld.Modules.Configurations;

namespace TosserWorld.Modules
{
    /// <summary>
    /// Base module class. A module is a custom component that adds functionality to an entity.
    /// </summary>
    public abstract class Module
    {
        public Entity Owner;


        public virtual void Update()
        {
            // This function will be called in the Owner's Update() function
        }

        public virtual void LateUpdate()
        {
            // This function will be called in the Owner's LateUpdate() function
        }

        public void Initialize(Entity entity, ModuleConfiguration configuration)
        {
            Owner = entity;
            OnInitialize(configuration);
        }

        protected virtual void OnInitialize(ModuleConfiguration configuration)
        {
            // This function will be called in the Owner's Start() function
        }
    }
}
