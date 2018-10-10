using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brain
{
    public abstract class BrainComponent : MonoBehaviour
    {
        protected class BrainAwareness
        {
            private List<BaseEntity> Awareness = new List<BaseEntity>();
            private AwarenessController Controller;
            private BrainComponent Owner;

            public BrainAwareness(BrainComponent owner)
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
            public BaseEntity Find(EntityTags tag = EntityTags.Any)
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
            public List<BaseEntity> FindAll(EntityTags tag = EntityTags.Any)
            {
                List<BaseEntity> list = new List<BaseEntity>();

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
            public BaseEntity FindNearest(EntityTags tag = EntityTags.Any)
            {
                BaseEntity nearest = null;
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


            private void Add(BaseEntity entity)
            {
                Debug.Log("Detected: " + entity.name);
                Awareness.Add(entity);
            }

            private void Remove(BaseEntity entity)
            {
                Awareness.Remove(entity);
                Debug.Log("Lost: " + entity.name);
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

        public float AwarenessRadius = 10;

        public bool IsContained { get; private set; }

        protected NPCEntity Owner;
        protected BrainAwareness Awareness;

        public BrainTriggers Triggers { get; private set; }

        private Coroutine CurrentLoop;

        private TextMesh SpeechBubble;
        private int SpeechStack = 0;

        void Awake()
        {
            Owner = GetComponent<NPCEntity>();

            Awareness = new BrainAwareness(this);
            Triggers = new BrainTriggers();
        }

        void Start()
        {
            CreateSpeechBubble();
        }

        void Update()
        {
            if (CurrentLoop == null)
            {
                CurrentLoop = StartCoroutine(IsContained ? InternalContainerLoop() : InternalMainLoop());
            }
        }

        private void CreateSpeechBubble()
        {
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(transform, false);
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

        protected abstract IEnumerator MainLoop();
        protected virtual IEnumerator ContainerLoop()
        {
            yield return null;
        }

        public void SetContained(bool contained)
        {
            IsContained = contained;
            StopCurrentLoop();
        }

        private void StopCurrentLoop()
        {
            StopCoroutine(CurrentLoop);
            CurrentLoop = null;
        }


        //// ---- SECONDARY ACTIONS ----

        protected void Talk(string line)
        {
            StartCoroutine(InternalTalk(line));
        }


        //// ---- MAIN ACTIONS ----

        protected bool GoTo(Vector2 destination)
        {
            Vector2 position = transform.position;

            if (position != destination)
            {
                // Calculate the needed walk to reach the destination
                Vector2 walk = destination - position;

                if (walk.magnitude > Owner.WalkDelta)
                {
                    // If the distance remaining is greater than the NPC's walk delta, walk at full speed
                    Owner.Walk(walk);
                }
                else
                {
                    // If else, close the gap and stop movement
                    transform.position = destination;
                    Owner.Stop();
                }

                return true;
            }

            return false;
        }

        protected bool Leash(BaseEntity target, float near, float far)
        {
            Vector2 position = transform.position;
            Vector2 destination = target.transform.position;

            // Calculate the needed walk to reach the target
            Vector2 walk = destination - position;
            if (walk.magnitude > far)
            {
                // Within go threshold
                Owner.Walk(walk);
            }
            else if (walk.magnitude < near)
            {
                // Within stop threshold
                Owner.Stop();
            }

            return true;
        }

        //// ---- INTERNAL COROUTINES ----

        private IEnumerator InternalMainLoop()
        {
            while (true)
            {
                yield return MainLoop();
            }
        }

        private IEnumerator InternalContainerLoop()
        {
            while (true)
            {
                yield return ContainerLoop();
            }
        }

        private IEnumerator InternalTalk(string line)
        {
            SpeechStack++;
            SpeechBubble.text = line;
            yield return new WaitForSeconds(1);
            if (--SpeechStack == 0)
            {
                SpeechBubble.text = null;
            }
        }

        //// ---- OTHER ----
    }
}
