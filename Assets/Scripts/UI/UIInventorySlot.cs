using UnityEngine;
using UnityEngine.UI;

using TosserWorld.Modules;
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
            Entity item = Inventory.Peek(Slot);

            if (item == null)
            {
                SlotImage.sprite = null;
                SlotAmount.text = null;
            }
            else
            {
                SlotImage.sprite = item.InventorySprite;

                if (item.GetModule<StackingModule>() != null)
                {
                    SlotAmount.text = item.GetModule<StackingModule>().Amount.ToString();
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
                    UICursor.Cursor.SetAttachedEntity(Inventory.Place(UICursor.Cursor.AttachedEntity, Slot));
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
                    if (UICursor.Cursor.AttachedEntity.MatchStacks(Inventory.Peek(Slot)))
                    {
                        var cursorStack = UICursor.Cursor.AttachedEntity.GetModule<StackingModule>();

                        if (!cursorStack.IsMaxed)
                            cursorStack.CombineStack(Inventory.TakeSingle(Slot));
                    }
                }
            }
        }
    }
}
