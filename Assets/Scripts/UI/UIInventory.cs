using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Entity;
using Entity.Type;
using Entity.Component;

public class UIInventory : MonoBehaviour
{
    private static GameObject SlotPrefab;


    private ContainerComponent Container;

    void Awake()
    {
        if (SlotPrefab == null)
        {
            SlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/InventorySlot");
        }
    }

    // Use this for initialization
    void Start ()
    {
	}
	
    public void CreateInventoryGrid(ContainerComponent container)
    {
        Container = container;

        int slotCount = Container.SlotCount;

        for (int i = 0; i < slotCount; ++i)
        {
            int x = i % Container.Cols;
            int y = i / Container.Cols;

            GameObject newSlot = Instantiate(SlotPrefab);
            newSlot.transform.SetParent(transform);

            newSlot.GetComponent<UIInventorySlot>().Storage = Container.Storage;
            newSlot.GetComponent<UIInventorySlot>().Slot = i;

            Vector2 pos = new Vector2(10 + (x * 60), -10 - (y * 60));
            newSlot.transform.localPosition = pos;
        }
        Vector2 panelSize = new Vector2(10 + (Container.Cols * 60), 10 + (Container.Rows * 60));
        GetComponent<RectTransform>().sizeDelta = panelSize;
    }

	// Update is called once per frame
	void Update () {
	}
}
