using UnityEngine;

namespace TosserWorld.Modules
{
    [CreateAssetMenu(fileName = "New Stacking Module", menuName = "Modules/Stacking")]
    public class StackingModule : Module
    {
        public int Amount { get; private set; }

        public int MaxAmount;

        public bool IsStackable;


        public StackingModule()
        {
            Amount = 1;
            MaxAmount = 64;
            IsStackable = true;
        }

        protected override Module Clone()
        {
            StackingModule clone = CreateInstance<StackingModule>();

            clone.MaxAmount = MaxAmount;
            clone.IsStackable = IsStackable;

            return clone;
        }

        /// <summary>
        /// Combines a stack into this one and returns a leftover stack if there is any.
        /// </summary>
        /// <param name="other">The stack to combine</param>
        /// <returns>Leftover stack (may be null)</returns>
        public Entity CombineStack(Entity other)
        {
            StackingModule otherStack = other.GetModule<StackingModule>();

            // Combine the two stacks
            Amount += otherStack.Amount;

            if (Amount > MaxAmount)
            {
                // Cap the amount to the value in MaxAmount and spill it over back to the original stack
                otherStack.Amount = MaxAmount - Amount;
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
