using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TosserWorld;
using TosserWorld.Modules;

public class UIInventorySlot : MonoBehaviour {

    public InventorySpace Storage { get; set; }
    public int Slot { get; set; }

    private Image SlotImage;
    private Text SlotAmount;

    // Use this for initialization
    void Start ()
    {
        SlotImage = transform.GetChild(0).GetComponent<Image>();
        SlotAmount = transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        Entity item = Storage[Slot];

        if (item == null)
        {
            SlotImage.sprite = null;
            SlotAmount.text = null;
        }
        else
        {
            SlotImage.sprite = item.InventorySprite;

            if (item.GetModule<Stacker>() != null)
            {
                SlotAmount.text = item.GetModule<Stacker>().Amount.ToString();
            }
        }
	}
}
