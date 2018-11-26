using System.Collections.Generic;
using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New TagList", menuName = "Modules/Tag List")]
    public class TagList : Module
    {
        // Tags
        public List<EntityTags> Tags;


        public TagList()
        {
            Tags = new List<EntityTags>();
        }

        protected override Module Clone()
        {
            TagList clone = CreateInstance<TagList>();

            clone.Tags = new List<EntityTags>(Tags);

            return clone;
        }
    }
}
