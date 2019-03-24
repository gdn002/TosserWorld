using TosserWorld.Entities;
using UnityEngine;

namespace TosserWorld.Modules.ActionScripts
{
    public class PokeAction : ActionScript
    {
        public override void Run(Entity actor)
        {
            Entity entity = actor.Brain.Awareness.FindNearest();

            if (entity != null)
            {
                if (entity.DistanceTo(actor) < 1.5f)
                    Debug.Log(entity.Name + " has been poked by " + actor.Name + " with " + Owner.Name + ".");
                else
                    Debug.Log("Nothing to poke.");
            }
        }
    }
}
