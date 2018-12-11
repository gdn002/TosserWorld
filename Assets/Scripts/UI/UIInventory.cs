using UnityEngine;
using TosserWorld.Modules;

public class UIInventory : MonoBehaviour
{
    private static GameObject SlotPrefab;


    private ContainerModule Inventory;

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
	
    public void CreateInventoryGrid(ContainerModule inventory)
    {
        Inventory = inventory;

        int slotCount = Inventory.SlotCount;

        for (int i = 0; i < slotCount; ++i)
        {
            int x = i % Inventory.Cols;
            int y = i / Inventory.Cols;

            GameObject newSlot = Instantiate(SlotPrefab);
            newSlot.transform.SetParent(transform);

            newSlot.GetComponent<UIInventorySlot>().Storage = Inventory.Storage;
            newSlot.GetComponent<UIInventorySlot>().Slot = i;

            Vector2 pos = new Vector2(10 + (x * 60), -10 - (y * 60));
            newSlot.transform.localPosition = pos;
        }
        Vector2 panelSize = new Vector2(10 + (Inventory.Cols * 60), 10 + (Inventory.Rows * 60));
        GetComponent<RectTransform>().sizeDelta = panelSize;
    }

	// Update is called once per frame
	void Update () {
	}
}
