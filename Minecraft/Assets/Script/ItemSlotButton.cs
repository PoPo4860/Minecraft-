using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotButton : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerUpHandler, 
    IPointerDownHandler,
    IPointerExitHandler
{
    public int slotNumber;
    public enum ItemState { Empty, itemSlot, leftHandSlot, equipSlot, workSlot, workResultSlot};
    public ItemState item;
    // 이미지 누름
    public void OnPointerDown(PointerEventData eventData)
    {
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
