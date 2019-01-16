using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TosserWorld.UI.Panels
{
    public class UIPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static UIPanel InstantiatePanel(UIPanel prefab)
        {
            return Instantiate(prefab.gameObject).GetComponent<UIPanel>();
        }

        void Start()
        {
            OnStart();
        }

        void Update()
        {
            OnUpdate();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UIManager.Manager.BringToFront(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = transform.localPosition + (Vector3)eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}
