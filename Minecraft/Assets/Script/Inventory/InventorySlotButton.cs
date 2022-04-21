using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotButton : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerUpHandler, 
    IPointerDownHandler,
    IPointerExitHandler
{
    public int slotNumber;
    public bool slotInMouse = false;

    public enum EItemState { Empty, InventorySlot};
    public EItemState state;

    private void OnDisable()
    {
        slotInMouse = false;
    }
    private void Update()
    {
        if (state == EItemState.InventorySlot && true == Input.GetKeyDown(KeyCode.Q) && true == slotInMouse)
        {
            if (true == Input.GetKey(KeyCode.LeftShift))
                GameManager.Instance.playerInventory.DropItemFromInventoy(slotNumber, 64);
            else
                GameManager.Instance.playerInventory.DropItemFromInventoy(slotNumber, 1);
        }
    }

    // �̹��� ����
    public void OnPointerDown(PointerEventData eventData)
    {
        if (EItemState.InventorySlot == state)
        {
            if(PointerEventData.InputButton.Left == eventData.button)
                GameManager.Instance.playerInventory.LeftClickSlot(slotNumber);

            if (PointerEventData.InputButton.Right == eventData.button)
                GameManager.Instance.playerInventory.RightClickSlot(slotNumber);
        }
    }

    // �̹��� �� (�����ִ� ���¿��߸� ȣ�� �� ���� �̹������� ȣ��)
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    // �̹����� ���콺�� ������ ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        slotInMouse = true;
    }

    // �̹������� ���콺�� �־����� ȣ��(�����Ŀ� ȣ�Ⱑ��)
    public void OnPointerExit(PointerEventData eventData)
    {
        slotInMouse = false;
    }
}
