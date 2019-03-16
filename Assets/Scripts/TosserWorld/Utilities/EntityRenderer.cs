using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

using TosserWorld.Entities;

using System.Collections.Generic;

namespace TosserWorld.Utilities
{
    // Utility for ensuring sprites always face the camera and are sorted by world position accordingly
    public class EntityRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool EnableIsometry = true;
        public bool Selectable = true;

        private SortingGroup SortGroup;
        private List<SpriteRenderer> AttachedRenderers = new List<SpriteRenderer>();

        public Quaternion Rotation { get { return transform.rotation; } }
        public Entity Owner;

        void Start()
        {
            SortGroup = GetComponent<SortingGroup>();
            if (SortGroup != null)
            {
                SortGroup.sortingOrder = 1;
            }
            
            if (transform.parent != null)
            {
                Owner = transform.parent.GetComponent<Entity>();
                AttachedRenderers = Owner.GetComponentsInEntity<SpriteRenderer>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (EnableIsometry)
            {
                transform.rotation = CameraController.CameraRotation;
                //IsometricSorting();
            }
        }

        private void IsometricSorting()
        {
            // Sort sprites by rotating their world position by 45 degrees and measuring them along the X axis
            float position = CameraController.Controller.Orientation.RelativeVertical(transform.position);
            int sort = (int)(-position * 1000);

            if (SortGroup != null)
            {
                SortGroup.sortingOrder = sort;
            }
        }

        public void EnableRendering(bool enable = true)
        {
            foreach (var renderer in AttachedRenderers)
            {
                renderer.enabled = enable;
            }
        }

        public void ResetRotation()
        {
            transform.localRotation = Quaternion.identity;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            Owner.OnPointerExit(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Selectable)
            {
                Owner.OnPointerEnter(eventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selectable)
            {
                Owner.OnPointerClick(eventData);
            }
        }
    }
}
