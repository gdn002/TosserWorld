using UnityEngine;
using TosserWorld.Util;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Modules/Stats")]
    public class Stats : Module
    {
        public Stat Health;
        public Stat Stamina;
        public Stat Air;

        public Stats()
        {
            Health = new Stat(100);
            Stamina = new Stat(100);
            Air = new Stat(100);
        }

        protected override Module Clone()
        {
            Stats clone = CreateInstance<Stats>();

            clone.Health = new Stat(Health);
            clone.Stamina = new Stat(Stamina);
            clone.Air = new Stat(Air);

            return clone;
        }
    }
}
