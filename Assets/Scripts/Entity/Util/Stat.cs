using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TosserWorld.Util
{
    public struct Stat
    {
        public Stat(int max, bool locked = true)
        {
            Current = max;
            Maximum = max;
            Modifier = 0;
            Locked = locked;
        }

        public Stat(Stat other)
        {
            Current = other.Current;
            Maximum = other.Maximum;
            Modifier = other.Modifier;
            Locked = other.Locked;
        }

        public int Current { get; private set; }
        public int Maximum { get; private set; }
        public int Modifier { get; private set; }

        public int EffectiveMaximum { get { return Maximum + Modifier; } }

        public bool Locked { get; private set; }

        public void Modify(int delta)
        {
            if (Locked)
            {
                Current += delta;
                if (Current < 0)
                    Current = 0;
                else if (Current > EffectiveMaximum)
                    Current = EffectiveMaximum;
            }
        }
    }
}
