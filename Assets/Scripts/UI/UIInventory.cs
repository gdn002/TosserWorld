using UnityEngine;
using TosserWorld.Modules;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using TosserWorld.UI.Panels;

namespace TosserWorld.UI
{
    public class UIInventory : UIPanel
    {
        private static GameObject SlotPrefab;


        private InventoryModule Inventory;
        private Text Title;
        private GameObject SlotArea;

        void Awake()
        {
            if (SlotPrefab == null)
            {
                SlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/InventorySlot");
            }
        }

        // Use this for initialization
        void Start()
        {
            
        }

        public void CreateInventoryGrid(InventoryModule inventory)
        {
            Inventory = inventory;

            Title = transform.Find("Title").GetComponent<Text>();
            SlotArea = transform.Find("SlotArea").gameObject;

            Title.text = Inventory.Owner.Name + "'s Inventory";

            Vector2 panelSize = new Vector2(10 + (Inventory.Cols * 60), 30 + (Inventory.Rows * 60));
            GetComponent<RectTransform>().sizeDelta = panelSize;

            int slot = 0;
            for (int x = 0; x < Inventory.Rows; ++x)
            {
                for (int y = 0; y < Inventory.Cols; ++y)
                {
                    GameObject newSlot = Instantiate(SlotPrefab);
                    newSlot.transform.SetParent(SlotArea.transform);

                    newSlot.GetComponent<UIInventorySlot>().Inventory = Inventory;
                    newSlot.GetComponent<UIInventorySlot>().Slot = slot;

                    Vector2 pos = new Vector2((y * 60), -(x * 60));
                    newSlot.transform.localPosition = pos;

                    ++slot;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
