using System.Collections.Generic;
using UnityEngine;

using TosserWorld.UI.Panels;

namespace TosserWorld.UI
{
    /// <summary>
    /// Handles the UI space
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Manager { get; private set; }

        private List<UIPanel> ActivePanels = new List<UIPanel>();


        public Canvas UICanvas { get; private set; }

        void Awake()
        {
            Manager = this;
        }

        void Start()
        {
            UICanvas = GetComponent<Canvas>();
        }


        public void AddPanel(UIPanel panel)
        {
            panel.transform.SetParent(transform, false);
            PlaceOnNextAvailableArea(panel);

            ActivePanels.Add(panel);

            BringToFront(panel);
        }

        public void RemovePanel(UIPanel panel)
        {
            ActivePanels.Remove(panel);
            Destroy(panel.gameObject);
        }

        public void BringToFront(UIPanel panel)
        {
            panel.transform.SetAsLastSibling();
        }

        private void PlaceOnNextAvailableArea(UIPanel nextPanel)
        {
            var nextRect = nextPanel.GetComponent<RectTransform>();
            Vector2 position;

            /*
            if (ActivePanels.Count == 0)
            {
                position = new Vector2(50, -50);
            }
            else
            {
                var canvasRect = GetComponent<RectTransform>();
                var lastRect = ActivePanels[ActivePanels.Count - 1].GetComponent<RectTransform>();

                if (lastRect.anchoredPosition.x + lastRect.rect.width + nextRect.rect.width < canvasRect.rect.width)
                    position = new Vector2(lastRect.anchoredPosition.x + lastRect.rect.width + 50, lastRect.anchoredPosition.y);
                else
                    position = new Vector2(50, lastRect.anchoredPosition.y + lastRect.rect.height - 50);
            }
            */

            position = new Vector2(50, -50) * (ActivePanels.Count + 1);
            nextRect.anchoredPosition = position;
        }
    }
}
