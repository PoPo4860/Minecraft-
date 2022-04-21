using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ItemSlot[] itemSlot;
    public PlayerQuickSlot playerQuickSlot;

    private const int slotInMaxItem = 64;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddInventoryItem(in int itemCode, in int itemNum, bool quickPriority = true)
    {
        if (true == CheckCanAddInventory(itemCode, out int slotNum, quickPriority))
        {
            if (CodeData.BLOCK_AIR == itemSlot[slotNum].itemCode)
            {   // 해당 위치가 비어있다면
                itemSlot[slotNum].itemCode = itemCode;
                itemSlot[slotNum].itemNum = itemNum;
                AddInventoryItem(itemSlot[slotNum], itemCode, quickPriority);
            }
            else
            {   // 해당 위치에 아이템이 있다면
                itemSlot[slotNum].itemNum += itemNum;
                AddInventoryItem(itemSlot[slotNum], itemCode, quickPriority);
            }
        }
        SetQuickSlot(slotNum);
    }
    private void AddInventoryItem(ItemSlot itemSlot, int itemCode, bool quickPriority = true)
    {
        int remainItem = itemSlot.itemNum - 64;
        if (0 < remainItem)
        {
            itemSlot.itemNum = 64;
            AddInventoryItem(itemCode, remainItem, quickPriority);
        }
    }
    public bool CheckCanAddInventory(in int itemCode, out int itemSlotNum, bool quickPriority)
    {
        itemSlotNum = -1;
        for (int slotNum = 0; slotNum < 36; ++slotNum)
        {
            if (itemCode == itemSlot[slotNum].itemCode && itemSlot[slotNum].itemNum < slotInMaxItem)
            {
                itemSlotNum = slotNum;
                return true;
            }
        }

        if (false == quickPriority)
        {
            for (int slotNum = 0; slotNum < 36; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }
        }
        else
        {
            for (int slotNum = 27; slotNum < 36; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }

            for (int slotNum = 0; slotNum < 27; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }
        }

        return false;
    }
    public void SetQuickSlot(int slotNum)
    {
        if(27 <= slotNum)
        {
            playerQuickSlot.itemSlot[slotNum - 27].itemCode = itemSlot[slotNum].itemCode;
            playerQuickSlot.itemSlot[slotNum - 27].itemNum = itemSlot[slotNum].itemNum;
        }
    }
    public void RightClickQuickSlotItem(int slotNum, out int itemCdoe, out int itemNum)
    {
        slotNum += 27;
        itemCdoe = itemSlot[slotNum].itemCode;
        itemNum = itemSlot[slotNum].itemNum;
        --itemSlot[slotNum];
        SetQuickSlot(slotNum);
    }
    public void LeftClickSlot(int slotNum)
    {
        if (true == MouseItemSlot.Instance.itemSlot.empty &&
            true == itemSlot[slotNum].empty)
            return;

        if (false == Input.GetKey(KeyCode.LeftShift))
        {
            // 슬롯간 교환이 일어나야 할때
            if (true == MouseItemSlot.Instance.itemSlot.empty ||
                true == MouseItemSlot.Instance.itemSlot.isMax ||
                true == itemSlot[slotNum].empty ||
                true == itemSlot[slotNum].isMax ||
                itemSlot[slotNum] != MouseItemSlot.Instance.itemSlot)
                MouseItemSlot.Instance.SwapItemSlot(ref itemSlot[slotNum]);

            // 슬롯에 아이템을 추가해야 할때
            if (itemSlot[slotNum] == MouseItemSlot.Instance.itemSlot)
            {
                int canAddItemNum = 64 - itemSlot[slotNum].itemNum;
                if (MouseItemSlot.Instance.itemSlot.itemNum <= canAddItemNum)
                {
                    itemSlot[slotNum] += MouseItemSlot.Instance.itemSlot;
                    MouseItemSlot.Instance.ClearItemSlot();
                }
                else
                {
                    itemSlot[slotNum].itemNum = 64;
                    MouseItemSlot.Instance.itemSlot -= canAddItemNum;
                }
            }
        }
        else
        {
            int itemCode = itemSlot[slotNum].itemCode;
            int itemNum = itemSlot[slotNum].itemNum;
            itemSlot[slotNum].Clear();
            if (slotNum < 27)
                AddInventoryItem(itemCode, itemNum, true);
            else
                AddInventoryItem(itemCode, itemNum, false);
        }
        SetQuickSlot(slotNum);
    }
    public void RightClickSlot(int slotNum)
    {
        if (true == MouseItemSlot.Instance.itemSlot.empty  &&
            1 != itemSlot[slotNum].itemNum)
        {
            int temp = (1 == itemSlot[slotNum].itemNum % 2) ? 1 : 0;
            int itemNum = itemSlot[slotNum].itemNum /= 2;
            itemSlot[slotNum].itemNum = itemNum + temp;
            if (0 != itemNum)
                MouseItemSlot.Instance.itemSlot.itemCode = itemSlot[slotNum].itemCode;

            MouseItemSlot.Instance.itemSlot.itemNum = itemNum;
        }
        else
        {
            if (true == itemSlot[slotNum].empty)
            {
                itemSlot[slotNum].itemCode = MouseItemSlot.Instance.itemSlot.itemCode;
                ++itemSlot[slotNum];
                --MouseItemSlot.Instance.itemSlot;
            }

            else if (false == itemSlot[slotNum].isMax && MouseItemSlot.Instance.itemSlot == itemSlot[slotNum])
            {
                ++itemSlot[slotNum];
                --MouseItemSlot.Instance.itemSlot;
            }
        }
        SetQuickSlot(slotNum);

    }
    public void DropItemFromInventoy(int itemSlotNum, int dropItemNum)
    {
        if (true== itemSlot[itemSlotNum].empty)
            return;

        int itemCode = itemSlot[itemSlotNum].itemCode;
        int itemNum = itemSlot[itemSlotNum].itemNum;
        Vector3 vec =  GameManager.Instance.player.cameraTransform.position;
        Vector3 vec2 = GameManager.Instance.player.cameraTransform.forward / 1.5f;
        GameManager.Instance.itemManager.AddDropItem(itemCode, dropItemNum, vec, vec2);
    }
}
