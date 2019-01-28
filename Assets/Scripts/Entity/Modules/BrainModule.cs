using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TosserWorld.Modules.BrainScripts;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Brain Module", menuName = "Modules/Brain")]
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
            /// <returns>The first entity that matches the tag, or null if none was found.</returns>
            public Entity Find(EntityTags tag = EntityTags.Any)
            {
                foreach (var entity in Awareness)
                {
                    if (entity.HasTag(tag))
                        return entity;
                }

                return null;
            }

            /// <summary>
            /// Finds all entities that matches the tag.
            /// </summary>
            /// <param name="tag">Tag to filter.</param>
            /// <returns>A list of entities that match the tag, or an empty list if none were found.</returns>
            public List<Entity> FindAll(EntityTags tag = EntityTags.Any)
            {
                List<Entity> list = new List<Entity>();

                foreach (var entity in Awareness)
                {
                    if (entity.HasTag(tag))
                        list.Add(entity);
                }

                return list;
            }

            /// <summary>
            /// Finds the nearest entity that matches the tag.
            /// </summary>
            /// <param name="tag">Tag to filter.</param>
            /// <returns>The first entity that matches the tag, or null if none was found.</returns>
            public Entity FindNearest(EntityTags tag = EntityTags.Any)
            {
                Entity nearest = null;
                float nearestDistance = float.MaxValue;

                foreach (var entity in Awareness)
                {
                    if (entity.HasTag(tag))
                    {
                        float distance = entity.DistanceTo(Owner.Owner);
                        if (distance < nearestDistance)
                        {
                            nearest = entity;
                            nearestDistance = distance;
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


        public int SelectedBrainScript = 0;
        private BrainScript ActiveBrain;

        public float AwarenessRadius = 5;

        public bool IsContained { get; private set; }


        public BrainAwareness Awareness;

        public BrainTriggers Triggers { get; private set; }


        private TextMesh SpeechBubble;
        private int SpeechStack = 0;


        protected override Module Clone()
        {
            BrainModule clone = CreateInstance<BrainModule>();

            clone.AwarenessRadius = AwarenessRadius;
            clone.SelectedBrainScript = SelectedBrainScript;

            return clone;
        }

        public void Run()
        {
            if (IsContained)
            {
                ActiveBrain.RunContainerLoop();
            }
            else
            {
                ActiveBrain.RunMainLoop();
            }
        }

        protected override void OnInitialize()
        {
            Awareness = new BrainAwareness(this);
            Triggers = new BrainTriggers();

            ActiveBrain = BrainScriptSelector.InstantiateScript(SelectedBrainScript);
            ActiveBrain.SetComponent(this);

            CreateSpeechBubble();

            Run();
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

        public void SetContained(bool contained)
        {
            IsContained = contained;
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
