using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Entity;

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
        EntityStack myStack = Storage[Slot];

        if (myStack == null)
        {
            SlotImage.sprite = null;
            SlotAmount.text = null;
        }
        else
        {
            SlotImage.sprite = myStack.Entity.InventorySprite;
            SlotAmount.text = myStack.Amount.ToString();
        }
	}
}
