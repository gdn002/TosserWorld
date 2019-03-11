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
    }


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
