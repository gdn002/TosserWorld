using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Entity.Type;

namespace Entity
{
    public class EntityStack : MonoBehaviour
    {
        public BaseEntity Entity { get; set; }
        public int Amount { get; set; }

        void Awake()
        {
            Entity = GetComponent<BaseEntity>();
            Amount = 1;
        }

        /// <summary>
        /// Combines the stack into this one and returns a leftover stack if there's any.
        /// </summary>
        /// <param name="stack">The stack to combine</param>
        /// <returns>Leftover stack (can be null)</returns>
        public EntityStack CombineStack(EntityStack stack)
        {
            // Combine the two stacks
            Amount += stack.Amount;

            // If the stack was completely merged, destroy its object
            Destroy(stack.gameObject);

            // Return any leftover stack
            return null;
        }
    }
}
