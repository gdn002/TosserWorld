using System.Collections.Generic;
using TosserWorld.Modules.Configurations;

namespace TosserWorld.Modules
{
    public enum EntityTags
    {
        Any = 0,

        // General
        Player = 1,

        // Material
        Sponge = 101,

        // Items
        Item = 201,
        Weapon,
        Food,
        Healing,
        Tool,
    }


    /// <summary>
    /// Module that contains an entity's tags. All entities have a tag list module, if a custom tag list is not provided a default tag list will be generated for the entity.
    /// </summary>
    public class TagListModule : Module
    {
        public List<EntityTags> Tags = new List<EntityTags>();

        public bool HasTag(EntityTags tag)
        {
            if (tag == EntityTags.Any)
                return true;

            return Tags.Contains(tag);
        }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            if (configuration != null)
            {
                TagListConfig tagListConfig = configuration as TagListConfig;
                Tags = new List<EntityTags>(tagListConfig.Tags);
            }
            else
            {
                Tags = new List<EntityTags>();
            }
        }
    }
}
