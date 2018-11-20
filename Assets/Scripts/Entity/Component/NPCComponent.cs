using Entity.Type;

namespace Entity.Component
{
    public abstract class NPCComponent : BasicComponent
    {
        public new NPCEntity Owner { get { return base.Owner as NPCEntity; } }
    }
}
