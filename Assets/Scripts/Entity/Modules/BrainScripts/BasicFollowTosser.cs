using System.Collections;
using UnityEngine;

namespace TosserWorld.Modules.BrainScripts
{
    public class BasicFollowTosser : BrainScript
    {
        public override void RunBehaviorTree()
        {
            if (Me.IsChild)
                return;

            Entity friend = Awareness.Find("Duck");
            if (friend != null)
            {
                Leash(friend, 0.5f, 1.5f);
                return;
            }

            if (Me.DistanceTo(PlayerEntity.Player) < 4)
            {
                Leash(PlayerEntity.Player, 1.5f, 2);
                return;
            }

            Stop();
        }
    }
}
