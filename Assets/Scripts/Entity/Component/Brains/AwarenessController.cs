using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brains
{
    public class AwarenessController : MonoBehaviour
    {
        public delegate void AwarenessCallback(BasicEntity entity);

        private AwarenessCallback EntityDetected;
        private AwarenessCallback EntityLost;

        public bool Active = false;

        public void Initialize(NPCBrain owner, AwarenessCallback onDetect, AwarenessCallback onLost)
        {
            EntityDetected = onDetect;
            EntityLost = onLost;

            gameObject.name = "Awareness";

            // Set this object as a child of the owner brain
            gameObject.transform.SetParent(owner.Owner.transform, false);

            // Set it to ignore raycast 
            gameObject.layer = LayerMask.NameToLayer("Awareness");

            // Add a kinematic rigidbody
            var rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.isKinematic = true;

            // Add a trigger collider to detect objects
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = owner.AwarenessRadius;

            // Awareness is active
            Active = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Active)
            {
                if (collision.gameObject != transform.parent.gameObject)
                {
                    var entity = collision.gameObject.GetComponent<BasicEntity>();
                    if (entity != null)
                    {
                        EntityDetected(entity);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entity = collision.gameObject.GetComponent<BasicEntity>();
            if (entity != null)
            {
                EntityLost(entity);
            }
        }
    }
}
