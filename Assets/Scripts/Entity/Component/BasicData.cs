using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Entity.Component
{
    [CreateAssetMenu(fileName = "New Basic Data", menuName = "Components/Basic/Data")]
    public class BasicData : BasicComponent
    {
        // Name
        public string Name;

        // Tags
        public List<EntityTags> Tags;


        public BasicData()
        {
            Name = "EntityName";
            Tags = new List<EntityTags>();
        }

        protected override BasicComponent Clone()
        {
            BasicData clone = CreateInstance<BasicData>();

            clone.Name = Name;
            clone.Tags = Tags;

            return clone;
        }
    }
}
