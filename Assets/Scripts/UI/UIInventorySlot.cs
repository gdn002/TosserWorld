using UnityEngine;
using UnityEngine.UI;

using TosserWorld.Modules;
using TosserWorld.Entities;
using UnityEngine.EventSystems;

namespace TosserWorld.UI
{
    public class UIInventorySlot : MonoBehaviour, IPointerClickHandler
    {

        public ContainerModule Inventory { get; set; }
        public int Slot { get; set; }

        private Image SlotImage;
        private Text SlotAmount;

        // Use this for initialization
        void Start()
        {
            SlotImage = transform.GetChild(0).GetComponent<Image>();
            SlotAmount = transform.GetChild(1).GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            StackingModule item = Inventory.Peek(Slot);

            if (item == null)
            {
                SlotImage.sprite = null;
                SlotAmount.text = null;
            }
            else
            {
                SlotImage.sprite = item.Owner.InventorySprite;

                if (item.IsStackable)
                {
                    SlotAmount.text = item.Amount.ToString();
                }
                else
                {
                    SlotAmount.text = null;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (UICursor.Cursor.AttachedEntity == null)
                {
                    UICursor.Cursor.SetAttachedEntity(Inventory.Take(Slot));
                }
                else
                {
                    var stack = UICursor.Cursor.AttachedEntity.Stacking;
                    if (stack != null)
                        UICursor.Cursor.SetAttachedEntity(Inventory.Place(stack, Slot));
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (UICursor.Cursor.AttachedEntity == null)
                {
                    UICursor.Cursor.SetAttachedEntity(Inventory.TakeHalf(Slot));
                }
                else
                {
                    var stack = UICursor.Cursor.AttachedEntity.Stacking;
                    if (stack != null)
                    {
                        if (stack.StacksMatch(Inventory.Peek(Slot)) && !stack.IsMaxed)
                        {
                            stack.CombineStack(Inventory.TakeSingle(Slot));
                        }
                    }
                }
            }
        }
    }
}
