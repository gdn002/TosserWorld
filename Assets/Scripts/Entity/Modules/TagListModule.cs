using System.Collections.Generic;
using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Tag List Module", menuName = "Modules/Tag List")]
    public class TagListModule : Module
    {
        // Tags
        public List<EntityTags> Tags;


        public TagListModule()
        {
            Tags = new List<EntityTags>();
        }

        protected override Module Clone()
        {
            TagListModule clone = CreateInstance<TagListModule>();

            clone.Tags = new List<EntityTags>(Tags);

            return clone;
        }
    }
}
