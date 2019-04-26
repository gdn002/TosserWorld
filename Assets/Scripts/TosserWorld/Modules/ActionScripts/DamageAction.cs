using TosserWorld.Entities;

namespace TosserWorld.Modules.ActionScripts
{
    public class DamageAction : ActionScript
    {
        public override void Run(Entity actor)
        {
            Entity entity = actor.Brain.Awareness.FindNearest();

            if (entity != null)
            {
                if (entity.DistanceTo(actor) < 1.5f)
                {
                    if (entity.Stats != null)
                    {
                        entity.Stats.Health.Modify(-15);

                        if (entity.Brain != null)
                        {
                            entity.Brain.Triggers.Set("Agressor", actor);
                        }
                    }
                }
            }
        }
    }
}
