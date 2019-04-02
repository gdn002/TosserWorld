using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TosserWorld.Utilities
{
    public struct Stat
    {
        public Stat(int max, bool enabled = true)
        {
            Current = max;
            Maximum = max;
            Modifier = 0;
            Enabled = enabled;
        }

        public Stat(Stat other)
        {
            Current = other.Current;
            Maximum = other.Maximum;
            Modifier = other.Modifier;
            Enabled = other.Enabled;
        }

        public int Current { get; private set; }
        public int Maximum { get; private set; }
        public int Modifier { get; private set; }

        public int EffectiveMaximum { get { return Maximum + Modifier; } }
        public bool IsMaxed { get { return Current >= Maximum; } }

        public bool Enabled { get; private set; }

        public void Modify(int delta)
        {
            if (Enabled)
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
