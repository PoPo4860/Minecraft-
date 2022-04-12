using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotButton : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerUpHandler, 
    IPointerDownHandler,
    IPointerExitHandler
{
    public int slotNumber;
    public enum ItemState { Empty, itemSlot};

    public ItemState state;
    // 이미지 누름
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ItemState.itemSlot == state)
            PlayerInventory.Instance.ClickSlot(slotNumber);
    }

    // 이미지 뗌 (눌러있는 상태여야만 호출 및 누른 이미지에서 호출)
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    // 이미지에 마우스가 닿으면 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    // 이미지에서 마우스가 멀어지면 호출(닿은후에 호출가능)
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
