using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;
using Entity.Component.Brains;

namespace Entity.Component
{
    [CreateAssetMenu(fileName = "New NPC Brain", menuName = "Components/NPC/Brain")]
    public class NPCBrain : NPCComponent
    {
        public class BrainAwareness
        {
            private List<BasicEntity> Awareness = new List<BasicEntity>();
            private AwarenessController Controller;
            private NPCBrain Owner;

            public BrainAwareness(NPCBrain owner)
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
            public BasicEntity Find(EntityTags tag = EntityTags.Any)
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
            public List<BasicEntity> FindAll(EntityTags tag = EntityTags.Any)
            {
                List<BasicEntity> list = new List<BasicEntity>();

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
            public BasicEntity FindNearest(EntityTags tag = EntityTags.Any)
            {
                BasicEntity nearest = null;
                float distance = float.MaxValue;

                foreach (var entity in Awareness)
                {
                    if (entity.HasTag(tag))
                    {
                        if (entity.DistanceTo(Owner.Owner) < distance)
                        {
                            nearest = entity;
                        }
                    }
                }

                return nearest;
            }

            public IEnumerator<BasicEntity> GetEnumerator()
            {
                foreach (var entity in Awareness)
                {
                    yield return entity;
                }
            }

            private void Add(BasicEntity entity)
            {
                Awareness.Add(entity);
                Owner.ActiveBrain.OnDetectEntity(entity);
            }

            private void Remove(BasicEntity entity)
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


        public Brain ActiveBrain;

        public float AwarenessRadius;

        public bool IsContained { get; private set; }


        public BrainAwareness Awareness;

        public BrainTriggers Triggers { get; private set; }


        private TextMesh SpeechBubble;
        private int SpeechStack = 0;


        public NPCBrain()
        {
            AwarenessRadius = 5;
        }

        protected override BasicComponent Clone()
        {
            NPCBrain clone = CreateInstance<NPCBrain>();

            clone.AwarenessRadius = AwarenessRadius;
            clone.ActiveBrain = Brain.Clone(ActiveBrain);

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

            if (ActiveBrain == null) ActiveBrain = new EmptyBrain();
            ActiveBrain.SetComponent(this);
            CreateSpeechBubble();

            Run();
        }

        private void CreateSpeechBubble()
        {
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(Owner.transform, false);
            var renderer = newObj.AddComponent<MeshRenderer>();
            SpeechBubble = newObj.AddComponent<TextMesh>();

            newObj.name = "Speech Bubble";
            newObj.tag = "IsometricSprite";
            newObj.transform.localPosition = new Vector3(0, 0, -1.5f);
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
