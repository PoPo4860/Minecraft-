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
    // �̹��� ����
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    // �̹��� �� (�����ִ� ���¿��߸� ȣ�� �� ���� �̹������� ȣ��)
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    // �̹����� ���콺�� ������ ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    // �̹������� ���콺�� �־����� ȣ��(�����Ŀ� ȣ�Ⱑ��)
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
