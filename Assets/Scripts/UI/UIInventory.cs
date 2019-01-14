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

        Vector2 panelSize = new Vector2(10 + (Inventory.Cols * 60), 10 + (Inventory.Rows * 60));
        GetComponent<RectTransform>().sizeDelta = panelSize;

        int slot = 0;
        for (int x = 0; x < Inventory.Rows; ++x)
        {
            for (int y = 0; y < Inventory.Cols; ++y)
            {
                GameObject newSlot = Instantiate(SlotPrefab);
                newSlot.transform.SetParent(transform);

                newSlot.GetComponent<UIInventorySlot>().Storage = Inventory.Storage;
                newSlot.GetComponent<UIInventorySlot>().Slot = slot;

                Vector2 pos = new Vector2(10 + (y * 60), -10 - (x * 60));
                newSlot.transform.localPosition = pos;

                ++slot;
            }
        }
    }

	// Update is called once per frame
	void Update () {
	}
}
