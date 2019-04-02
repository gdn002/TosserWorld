using TosserWorld.Modules.Configurations;
using TosserWorld.Entities;

namespace TosserWorld.Modules
{
    public class StackingModule : Module
    {
        public int MaxAmount;

        public bool IsStackable { get { return MaxAmount > 1; } }
        public int Amount { get; private set; }
        public bool IsMaxed { get { return Amount >= MaxAmount; } }


        public StackingModule()
        {
            Amount = 1;
        }

        protected override void OnInitialize(ModuleConfiguration configuration)
        {
            StackingConfig stackingConfig = configuration as StackingConfig;
            MaxAmount = stackingConfig.MaxAmount;
        }

        /// <summary>
        /// Combines a stack into this one and returns a leftover stack if there is any.
        /// </summary>
        /// <param name="other">The stack to combine</param>
        /// <returns>Leftover stack (may be null)</returns>
        public StackingModule CombineStack(StackingModule other)
        {
            // If either stack is maxed out there's nothing to be done
            if (IsMaxed || other.IsMaxed)
                return other;

            // Combine the two stacks
            Amount += other.Amount;

            if (Amount > MaxAmount)
            {
                // Cap the amount to the value in MaxAmount and spill it over back to the original stack
                other.Amount = Amount - MaxAmount;
                Amount = MaxAmount;
            }
            else
            {
                // Remove the other entity if the stack was completely merged
                other.Owner.Remove();
                other = null;
            }

            return other;
        }

        public StackingModule TakeFromStack(int amount)
        {
            if (amount >= Amount)
                return this;

            Entity clone = Owner.Clone();
            Amount -= amount;
            StackingModule stack = clone.Stacking;
            stack.Amount = amount;
            return stack;
        }

        public void ChangeAmount(int delta)
        {
            Amount += delta;
            if (Amount <= 0)
            {
                Owner.Remove();
            }
        }

        public bool StacksMatch(StackingModule other)
        {
            if (other == null)
                return false;

            return (Owner.Name == other.Owner.Name);
        }
    }
}
