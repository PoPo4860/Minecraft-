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
    // �̹��� ����
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ItemState.itemSlot == state)
        {
            if(PointerEventData.InputButton.Left == eventData.button)
                PlayerInventory.Instance.LeftClickSlot(slotNumber);

            if (PointerEventData.InputButton.Right == eventData.button)
                PlayerInventory.Instance.RightClickSlot(slotNumber);


        }
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
