using System.Collections.Generic;
using TosserWorld.Entities;

namespace TosserWorld.Modules.ActionScripts
{
    public class ActionScriptSelector
    {
        private static List<string> Names = new List<string>();
        private static List<System.Type> Types = new List<System.Type>();

        static ActionScriptSelector()
        {
            // All current ActionScript implementations must be included here
            ActionScript("Poke", typeof(PokeAction));
            ActionScript("Damage", typeof(DamageAction));

        }

        private static void ActionScript(string name, System.Type type)
        {
            Names.Add(name);
            Types.Add(type);
        }

        public static string Name(int i)
        {
            return Names[i];
        }

        public static string[] AllNames()
        {
            string[] allNames = new string[Names.Count];
            Names.CopyTo(allNames);
            return allNames;
        }

        public static ActionScript InstantiateScript(int i)
        {
            return System.Activator.CreateInstance(Types[i]) as ActionScript;
        }
    }

    public abstract class ActionScript
    {
        protected Entity Owner;

        public void Initialize(Entity owner)
        {
            Owner = owner;
        }

        public abstract void Run(Entity actor);
    }
}
