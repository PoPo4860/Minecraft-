using UnityEngine;

public class InventoryUI : ItemSlotUI
{
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            AddInventoryItem(CodeData.Item_DiamondPickaxe, 1);
            AddInventoryItem(CodeData.Item_StonePickaxe, 1);
            AddInventoryItem(CodeData.Item_WoodPickaxe, 1);
        }

    }
    public void AddInventoryItem(in int itemCode, in int itemNum, bool quickPriority = true)
    {
        if (true == CheckCanAddInventory(itemCode, out int slotNum, quickPriority))
        {
            if (CodeData.BLOCK_Air == itemSlot[slotNum].itemCode)
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
    public int RightClickQuickSlotItem(int slotNum)
    {
        slotNum += 27;
        int itemCode = itemSlot[slotNum].itemCode;
        if (null != CodeData.GetBlockInfo(itemCode))
            --itemSlot[slotNum];

        SetQuickSlot(slotNum);
        return itemCode;
    }
    public override void LeftClickSlot(int slotNum)
    {
        LeftClickSlotWork(slotNum);
        SetQuickSlot(slotNum);
    }
    public override void LeftShiftClickSlot(int slotNum)
    {
        LeftShiftClickSlotWork(slotNum);
        SetQuickSlot(slotNum);
    }
    public override void RightClickSlot(int slotNum)
    {
        RightClickSlotWork(slotNum);
        SetQuickSlot(slotNum);
    }

    public void DropItemFromInventoy(int itemSlotNum, int dropItemNum)
    {
        if (true== itemSlot[itemSlotNum].empty)
            return;

        int itemCode = itemSlot[itemSlotNum].itemCode;
        if(itemSlot[itemSlotNum].itemNum < dropItemNum)
            dropItemNum = itemSlot[itemSlotNum].itemNum;
        
        itemSlot[itemSlotNum] -= dropItemNum;
        Vector3 dropPos =  GameManager.Instance.player.cameraTransform.position;
        Vector3 dirVec = GameManager.Instance.player.cameraTransform.forward / 1.5f;
        GameManager.Instance.itemManager.AddDropItem(itemCode, dropItemNum, dropPos, dirVec);
        SetQuickSlot(itemSlotNum);
    }
}
