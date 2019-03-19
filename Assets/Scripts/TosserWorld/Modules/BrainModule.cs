using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TosserWorld.Modules.BrainScripts;
using TosserWorld.Entities;
using TosserWorld.Modules.Configurations;


namespace TosserWorld.Modules
{
    public class BrainModule : Module
    {
        public class BrainAwareness
        {
            private List<Entity> Awareness = new List<Entity>();
            private AwarenessController Controller;
            private BrainModule Owner;

            public BrainAwareness(BrainModule owner)
            {
                Owner = owner;

                GameObject obj = new GameObject();
                Controller = obj.AddComponent<AwarenessController>();

                Controller.Initialize(Owner, Add, Remove);
            }

            /// <summary>
            /// Finds the first entity that matches the tag. Priority is defined by the order the items were detected by the brain.
            /// </summary>
            /// <param name="tag">Tag to filter.</param>
            /// <param name="recursive">Whether to search recursively for childed entities.</param>
            /// <returns>The first entity that matches the tag, or null if none was found.</returns>
            public Entity Find(EntityTags tag = EntityTags.Any, bool recursive = false)
            {
                foreach (var entity in Awareness)
                {
                    if (entity.TagList.HasTag(tag))
                        return entity;

                    if (recursive)
                    {
                        Entity found = FindInInventory(entity.Inventory, tag);
                        if (found != null)
                            return found;
                    }
                }

                return null;
            }

            private Entity FindInInventory(InventoryModule inventory, EntityTags tag)
            {
                if (inventory == null)
                    return null;

                foreach (var item in inventory.Storage.GetContents())
                {
                    if (item.Owner.TagList.HasTag(tag))
                        return item.Owner;

                    Entity found = FindInInventory(item.Owner.Inventory, tag);
                    if (found != null)
                        return found;
                }

                return null;
            }

            /// <summary>
            /// Finds the first entity that matches a given name. Priority is defined by the order the items were detected by the brain.
            /// </summary>
            /// <param name="tag">Name to search.</param>
            /// <param name="recursive">Whether to search recursively for childed entities.</param>
            /// <returns>The first entity that matches the name, or null if none was found.</returns>
            public Entity Find(string name, bool recursive = false)
            {
                foreach (var entity in Awareness)
                {
                    if (entity.Name == name)
                        return entity;

                    if (recursive)
                    {
                        Entity found = FindInInventory(entity.Inventory, name);
                        if (found != null)
                            return found;
                    }
                }

                return null;
            }

            private Entity FindInInventory(InventoryModule inventory, string name)
            {
                if (inventory == null)
                    return null;

                foreach (var item in inventory.Storage.GetContents())
                {
                    if (item.Owner.Name == name)
                        return item.Owner;

                    Entity found = FindInInventory(item.Owner.Inventory, name);
                    if (found != null)
                        return found;
                }

                return null;
            }

            /// <summary>
            /// Finds all entities that matches the tag.
            /// </summary>
            /// <param name="tag">Tag to filter.</param>
            /// <returns>A list of entities that match the tag, or an empty list if none were found.</returns>
            public List<Entity> FindAll(EntityTags tag = EntityTags.Any, bool recursive = false)
            {
                List<Entity> list = new List<Entity>();

                foreach (var entity in Awareness)
                {
                    if (entity.TagList.HasTag(tag))
                        list.Add(entity);

                    if (recursive)
                        FindAllInInventory(entity.Inventory, list, tag);
                }

                return list;
            }

            private void FindAllInInventory(InventoryModule inventory, List<Entity> list, EntityTags tag)
            {
                if (inventory == null)
                    return;

                foreach (var item in inventory.Storage.GetContents())
                {
                    if (item.Owner.TagList.HasTag(tag))
                        list.Add(item.Owner);

                    FindAllInInventory(item.Owner.Inventory, list, tag);
                }
            }

            /// <summary>
            /// Finds the nearest entity that matches the tag.
            /// </summary>
            /// <param name="tag">Tag to filter.</param>
            /// <returns>The nearest entity that matches the tag, or null if none was found.</returns>
            public Entity FindNearest(EntityTags tag = EntityTags.Any, bool recursive = false)
            {
                Entity nearest = null;
                float nearestDistance = float.MaxValue;

                foreach (var entity in Awareness)
                {
                    float distance = entity.DistanceTo(Owner.Owner);

                    if (distance < nearestDistance)
                    {
                        if (entity.TagList.HasTag(tag))
                        {
                            nearest = entity;
                            nearestDistance = distance;
                        }
                        else if (recursive)
                        {
                            Entity found = FindInInventory(entity.Inventory, tag);
                            if (found != null)
                            {
                                nearest = found;
                                nearestDistance = distance;
                            }
                        }
                    }
                }

                return nearest;
            }

            /// <summary>
            /// Finds the nearest entity with a specific name.
            /// </summary>
            /// <param name="name">Name to search for.</param>
            /// <returns>The nearest entity that has the name, or null if none was found.</returns>
            public Entity FindNearest(string name, bool recursive = false)
            {
                Entity nearest = null;
                float nearestDistance = float.MaxValue;

                foreach (var entity in Awareness)
                {
                    float distance = entity.DistanceTo(Owner.Owner);

                    if (distance < nearestDistance)
                    {
                        if (entity.Name == name)
                        {
                            nearest = entity;
                            nearestDistance = distance;
                        }
                        else if (recursive)
                        {
                            Entity found = FindInInventory(entity.Inventory, name);
                            if (found != null)
                            {
                                nearest = found;
                                nearestDistance = distance;
                            }
                        }
                    }
                }

                return nearest;
            }

            public IEnumerator<Entity> GetEnumerator()
            {
                foreach (var entity in Awareness)
                {
                    yield return entity;
                }
            }

            private void Add(Entity entity)
            {
                Awareness.Add(entity);
                Owner.ActiveBrain.OnDetectEntity(entity);
                Debug.Log(Owner.Owner.Name + " detected " + entity.Name);
            }

            private void Remove(Entity entity)
            {
                Awareness.Remove(entity);
                Owner.ActiveBrain.OnLoseEntity(entity);
            }
        }

        public class BrainTriggers
        {
            private Dictionary<string, object> Triggers = new Dictionary<string, object>();

            public bool Contains(string name)
            {
                return Triggers.ContainsKey(name);
            }

            public object Get(string name)
            {
                return Triggers[name];
            }

            public T Get<T>(string name)
            {
                return (T)Triggers[name];
            }

            public void Set(string name, object value)
            {
                Triggers[name] = value;
            }

            public object Take(string name)
            {
                object value = Triggers[name];
                Clear(name);
                return value;
            }

            public T Take<T>(string name)
            {
                T value = (T)Triggers[name];
                Clear(name);
                return value;
            }

            public void Clear(string name)
            {
                Triggers.Remove(name);
            }

            public void ClearAll()
            {
                Triggers.Clear();
            }
        }

        private BrainScript ActiveBrain;

        public float AwarenessRadius;

        public BrainAwareness Awareness { get; private set; }
        public BrainTriggers Triggers { get; private set; }


        private TextMesh SpeechBubble;
        private int SpeechStack = 0;


        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            BrainConfig brainConfig = configuration as BrainConfig;
            AwarenessRadius = brainConfig.AwarenessRadius;

            Awareness = new BrainAwareness(this);
            Triggers = new BrainTriggers();

            ActiveBrain = BrainScriptSelector.InstantiateScript(brainConfig.SelectedBrainScript);
            ActiveBrain.SetComponent(this);

            CreateSpeechBubble();
        }

        public override void Update()
        {
            if (Owner.IsAlive)
            {
                ActiveBrain.RunBehaviorTree();
            }
        }

        private void CreateSpeechBubble()
        {
            // TODO: Speech bubbles need a rework

            GameObject newObj = new GameObject();
            newObj.transform.SetParent(Owner.transform, false);
            var renderer = newObj.AddComponent<MeshRenderer>();
            SpeechBubble = newObj.AddComponent<TextMesh>();

            newObj.name = "Speech Bubble";
            newObj.tag = "IsometricSprite";
            newObj.transform.localPosition = new Vector3(0, 1.5f, 0);
            newObj.transform.localScale = new Vector3(0.1f, 0.1f, 1);

            renderer.sortingLayerName = "Top";
            renderer.sortingOrder = 0;

            SpeechBubble.anchor = TextAnchor.MiddleCenter;
            SpeechBubble.alignment = TextAlignment.Center;
            SpeechBubble.fontSize = 40;
        }


        //// ---- INTERNAL COROUTINES ----

        public IEnumerator Talk(string line)
        {
            SpeechStack++;
            SpeechBubble.text = line;
            yield return new WaitForSeconds(1);
            if (--SpeechStack == 0)
            {
                SpeechBubble.text = null;
            }
        }
    }
}
