using UnityEngine;

namespace TosserWorld.UI
{
    /// <summary>
    /// Handles the UI space
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Manager { get; private set; }


        public Canvas UICanvas { get; private set; }

        void Awake()
        {
            Manager = this;
        }

        void Start()
        {
            UICanvas = GetComponent<Canvas>();
        }


        public void AddPanel(GameObject panel)
        {
            panel.transform.SetParent(UICanvas.transform, false);
        }
    }
}
