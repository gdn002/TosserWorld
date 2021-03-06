﻿using UnityEngine;
using System.Collections.Generic;

namespace TosserWorld.Modules.Configurations
{
    [CreateAssetMenu(fileName = "New Tag List Configuration", menuName = "Module Configurations/Tag List")]
    public class TagListConfig : ModuleConfiguration
    {
        public List<EntityTags> Tags = new List<EntityTags>();
    }
}
