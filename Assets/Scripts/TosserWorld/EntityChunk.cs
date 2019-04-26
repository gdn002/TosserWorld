using System.Collections.Generic;
using UnityEngine;
using TosserWorld.Entities;
using TosserWorld.Modules;

namespace TosserWorld
{
    public class EntityChunk
    {
        public static EntityChunk GlobalChunk = new EntityChunk();

        private Dictionary<EntityTags, LinkedList<Entity>> TagDictionary = new Dictionary<EntityTags, LinkedList<Entity>>();

        public EntityChunk()
        {
            TagDictionary.Add(EntityTags.Any, new LinkedList<Entity>());
        }

        /// <summary>
        /// Adds entity to the tag dictionary
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public void AddEntity(Entity entity)
        {
            // Add the entity to all its tags in the dictionary
            foreach (var tag in entity.TagList.Tags)
            {
                // Create dictionary entry if it doesn't exist
                if (!TagDictionary.ContainsKey(tag))
                {
                    TagDictionary.Add(tag, new LinkedList<Entity>());
                }

                // Add entity to dictionary entry
                TagDictionary[tag].AddLast(entity);
            }
            // Also add the entity to the "Any" entry
            TagDictionary[EntityTags.Any].AddLast(entity);
        }

        /// <summary>
        /// Removes entity from tag dictionary
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveEntity(Entity entity)
        {
            // Remove the entity to all its tags in the dictionary
            foreach (var tag in entity.TagList.Tags)
            {
                // Remove entity from dictionary entry
                TagDictionary[tag].Remove(entity);
            }

            // Also remove the entity from the "Any" entry
            TagDictionary[EntityTags.Any].Remove(entity);
        }

        /// <summary>
        /// Fetch all active entities that match a tag in this chunk
        /// </summary>
        /// <param name="tag">The tag to match</param>
        /// <returns>An array of matching entities, or null if none were found</returns>
        public Entity[] GetAllEntities(EntityTags tag = EntityTags.Any)
        {
            if (TagDictionary.ContainsKey(tag))
            {
                LinkedList<Entity> matches = TagDictionary[tag];
                if (matches.Count > 0)
                {
                    Entity[] returnArray = new Entity[matches.Count];
                    matches.CopyTo(returnArray, 0);
                    return returnArray;
                }
            }
            return null;
        }

        /// <summary>
        /// Fetch all active entities within a range that match a tag in this chunk
        /// </summary>
        /// <param name="entity">The entity to compare to</param>
        /// <param name="range">The maximum range</param>
        /// <param name="tag">The tag to match</param>
        /// <returns>An array of matching entities, or null if none were found</returns>
        public Entity[] GetAllEntitiesInRange(Entity entity, float range, EntityTags tag = EntityTags.Any)
        {
            if (TagDictionary.ContainsKey(tag))
            {
                LinkedList<Entity> matches = TagDictionary[tag];
                if (matches.Count > 0)
                {
                    LinkedList<Entity> entitiesInRange = new LinkedList<Entity>();
                    foreach (var match in matches)
                    {
                        if (match == entity)
                            continue;

                        float distance = Vector2.Distance(match.transform.position, entity.transform.position);
                        if (distance < range)
                        {
                            entitiesInRange.AddLast(match);
                        }
                    }

                    if (entitiesInRange.Count > 0)
                    {
                        Entity[] returnArray = new Entity[entitiesInRange.Count];
                        entitiesInRange.CopyTo(returnArray, 0);
                        return returnArray;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Fetch the nearest entity to a position that matches a tag in this chunk
        /// </summary>
        /// <param name="entity">The entity to compare to</param>
        /// <param name="tag">The tag to match</param>
        /// <returns>Nearest matching entity, or null if none were found</returns>
        public Entity GetNearestEntity(Entity entity, EntityTags tag = EntityTags.Any)
        {
            return GetNearestEntityInRange(entity, float.PositiveInfinity, tag);
        }

        /// <summary>
        /// Fetch the nearest entity to a position within a range that matches a tag in this chunk 
        /// </summary>
        /// <param name="entity">The entity to compare to</param>
        /// <param name="range">The maximum range</param>
        /// <param name="tag">The tag to match</param>
        /// <returns>Nearest matching entity, or null if none were found</returns>
        public Entity GetNearestEntityInRange(Entity entity, float range, EntityTags tag = EntityTags.Any)
        {
            Entity nearestEntity = null;

            if (TagDictionary.ContainsKey(tag))
            {
                LinkedList<Entity> matches = TagDictionary[tag];
                float nearestDistance = range;

                foreach (var match in matches)
                {
                    if (match == entity)
                        continue;

                    float distance = Vector2.Distance(match.transform.position, entity.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestEntity = match;
                        nearestDistance = distance;
                    }
                }
            }
            return nearestEntity;
        }
    }
}
