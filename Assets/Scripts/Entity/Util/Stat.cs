using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Util
{
    public struct Stat
    {
        public Stat(int max)
        {
            Current = max;
            Maximum = max;
            Modifier = 0;
        }

        public Stat(Stat other)
        {
            Current = other.Current;
            Maximum = other.Maximum;
            Modifier = other.Modifier;
        }

        public int Current { get; private set; }
        public int Maximum { get; private set; }
        public int Modifier { get; private set; }

        public int EffectiveMaximum { get { return Maximum + Modifier; } }

        public void Modify(int delta)
        {
            Current += delta;
            if (Current < 0)
                Current = 0;
            else if (Current > EffectiveMaximum)
                Current = EffectiveMaximum;
        }
    }
}
