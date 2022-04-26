using UnityEngine;
using UnityEngine.EventSystems;

public class SlotButton : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerUpHandler, 
    IPointerDownHandler,
    IPointerExitHandler
{
    public int slotNumber;
    private bool slotInMouse = false;
    public enum EItemState { Empty, Base, InventorySlot, CraftingSlot, CraftingResultSlot};
    public EItemState state;

    private ItemSlotUI itemSlotUI;
    private void Start()
    {
        switch(state)
        {
            case EItemState.InventorySlot:
                itemSlotUI = UIManager.Instance.playerInventoryUI;
                break;
            case EItemState.CraftingSlot:
                itemSlotUI = UIManager.Instance.inventoryCraftingUI;
                break;
        }
    }
    private void OnDisable()
    {
        slotInMouse = false;
    }
    private void Update()
    {
        if (state == EItemState.InventorySlot && true == Input.GetKeyDown(KeyCode.Q) && true == slotInMouse)
        {
            if (true == Input.GetKey(KeyCode.LeftShift))
                UIManager.Instance.playerInventory.DropItemFromInventoy(slotNumber, 64);
            else
                UIManager.Instance.playerInventory.DropItemFromInventoy(slotNumber, 1);
        }
    }

    // 이미지 누름
    public void OnPointerDown(PointerEventData eventData)
    {

        // 바깥에 아이템 버리기
        if (EItemState.Empty == state)
        {
            if (PointerEventData.InputButton.Left == eventData.button)
                GameManager.Instance.mouseItemSlot.DropItemFromInventoy(64);

            if (PointerEventData.InputButton.Right == eventData.button)
                GameManager.Instance.mouseItemSlot.DropItemFromInventoy(1);
            return;
        }
        if (EItemState.Base == state)
            return;

        if(false == Input.GetKey(KeyCode.LeftShift))
        {
            if (PointerEventData.InputButton.Left == eventData.button)
                itemSlotUI.LeftClickSlot(slotNumber);

            if (PointerEventData.InputButton.Right == eventData.button)
                itemSlotUI.RightClickSlot(slotNumber);
        }
        else
        {
            if (PointerEventData.InputButton.Left == eventData.button)
                itemSlotUI.LeftShiftClickSlot(slotNumber);
        }


    }

    // 이미지 뗌 (눌러있는 상태여야만 호출 및 누른 이미지에서 호출)
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    // 이미지에 마우스가 닿으면 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        slotInMouse = true;
    }

    // 이미지에서 마우스가 멀어지면 호출(닿은후에 호출가능)
    public void OnPointerExit(PointerEventData eventData)
    {
        slotInMouse = false;
    }
}
