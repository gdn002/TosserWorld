using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Entity.Type;

namespace Entity.Component
{
    public class BaseComponent : MonoBehaviour
    {
        public BaseEntity Owner;

        void Start()
        {
            Owner = GetComponent<BaseEntity>();

            OnStart();
        }

        void Update()
        {
            OnUpdate();
        }

        //// ---- COMPONENT BEHAVIOUR ----

        /// <summary>
        /// Implement component initial setup
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Implement component behaviour
        /// </summary>
        protected virtual void OnUpdate()
        {
        }
    }
}
