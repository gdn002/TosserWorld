using UnityEngine;
using Entity.Util;

namespace Entity.Component
{
    [CreateAssetMenu(fileName = "New NPC Stats", menuName = "Components/NPC/Stats")]
    public class NPCStats : NPCComponent
    {
        public Stat Health;
        public Stat Stamina;
        public Stat Air;

        public float Speed;

        // Max distance walked per frame
        public float FrameSpeed { get { return Speed * Time.fixedDeltaTime; } }


        public NPCStats()
        {
            Health = new Stat(100);
            Stamina = new Stat(100);
            Air = new Stat(100);

            Speed = 1;
        }

        protected override BasicComponent Clone()
        {
            NPCStats clone = CreateInstance<NPCStats>();

            clone.Health = new Stat(Health);
            clone.Stamina = new Stat(Stamina);
            clone.Air = new Stat(Air);

            clone.Speed = Speed;

            return clone;
        }
    }
}
