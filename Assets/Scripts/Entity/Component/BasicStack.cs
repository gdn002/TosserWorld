using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Entity.Type;

namespace Entity.Component
{
    [CreateAssetMenu(fileName = "New Basic Stack", menuName = "Components/Basic/Stack")]
    public class BasicStack : BasicComponent
    {
        public int Amount { get; private set; }

        public int MaxAmount;

        public bool Stackable;


        public BasicStack()
        {
            MaxAmount = 64;
            Stackable = true;
        }

        protected override BasicComponent Clone()
        {
            BasicStack clone = CreateInstance<BasicStack>();

            clone.MaxAmount = MaxAmount;
            clone.Stackable = Stackable;

            return clone;
        }

        /// <summary>
        /// Combines a stack into this one and returns a leftover stack if there is any.
        /// </summary>
        /// <param name="other">The stack to combine</param>
        /// <returns>Leftover stack (may be null)</returns>
        public BasicEntity CombineStack(BasicEntity other)
        {
            // Combine the two stacks
            Amount += other.Stack.Amount;

            if (Amount > MaxAmount)
            {
                // Cap the amount to the value in MaxAmount and spill it over back to the original stack
                other.Stack.Amount = MaxAmount - Amount;
                Amount = MaxAmount;
            }
            else
            {
                // Destroy the other entity if the stack was completely merged
                Destroy(other.gameObject);
                other = null;
            }

            return other;
        }
    }
}
