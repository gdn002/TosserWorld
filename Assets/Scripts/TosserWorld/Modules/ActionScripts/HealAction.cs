using UnityEngine;
using TosserWorld.Entities;

namespace TosserWorld.Modules.ActionScripts
{
    public class HealAction : ActionScript
    {
        public override void Run(Entity actor)
        {
            actor.Stats.Health.Modify(25);
            Debug.Log(actor.Name + " healed for 25 points");
        }
    }
}
