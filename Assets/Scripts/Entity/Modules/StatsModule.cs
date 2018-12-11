using UnityEngine;
using TosserWorld.Util;
using System.Collections.Generic;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Stats Module", menuName = "Modules/Stats")]
    public class StatsModule : Module
    {
        public class DEFNAMES
        {
            public static string HEALTH = "Health";
            public static string STAMINA = "Stamina";

            public static bool VerifyName(string name)
            {
                return name == HEALTH ||
                       name == STAMINA;
            }
        }

        private Dictionary<string, Stat> StatDictionary;

        public StatsModule()
        {
            StatDictionary = new Dictionary<string, Stat>();
        }

        protected override Module Clone()
        {
            StatsModule clone = CreateInstance<StatsModule>();

            clone.StatDictionary = new Dictionary<string, Stat>(StatDictionary);

            return clone;
        }


        public Stat this[string name]
        {
            get { return StatDictionary[name]; }
        }

        public void Add(string name, Stat stat)
        {
            StatDictionary.Add(name, stat);
        }

        public void Clear()
        {
            StatDictionary.Clear();
        }
    }
}
