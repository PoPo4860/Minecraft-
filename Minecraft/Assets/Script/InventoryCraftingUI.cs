using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCraftingUI : ItemSlotUI
{
    int[] resipe = new int[9];
    // [0][1][2]
    // [3][4][5]
    // [6][7][8]
    void SetResipe()
    {
        resipe[0] = itemSlot[0].itemCode;
        resipe[1] = itemSlot[1].itemCode;
        resipe[3] = itemSlot[2].itemCode;
        resipe[4] = itemSlot[3].itemCode;
        
        if (0 == resipe[0] && 0 == resipe[1])
        {
            IntSwap(ref resipe[0],ref resipe[3]);
            IntSwap(ref resipe[1],ref resipe[4]);
        }

        if (0 == resipe[0] && 0 == resipe[3])
        {
            IntSwap(ref resipe[0], ref resipe[1]);
            IntSwap(ref resipe[3], ref resipe[4]);
        }

        int itemCode = CraftingResipe.GetResipe(resipe); ;
        itemSlot[4].itemCode = itemCode;
        itemSlot[4].itemNum = (itemCode == 0) ? 0 : 1;
    }

    public override void LeftClickSlot(int slotNum)
    {
        if(4 != slotNum)
        {
            LeftClickSlotWork(slotNum);
        }
        else
        {
            if(true == GameManager.Instance.mouseItemSlot.itemSlot.empty)
            {
                GameManager.Instance.mouseItemSlot.SwapItemSlot(ref itemSlot[4]);
            }
            --itemSlot[0];
            --itemSlot[1];
            --itemSlot[2];
            --itemSlot[3];
        }
        SetResipe();
    }
    public override void LeftShiftClickSlot(int slotNum)
    {
        if (4 != slotNum)
        {
            LeftShiftClickSlotWork(slotNum);
        }
        else
        {
            while(0 != itemSlot[4].itemCode)
            {
                if (true == GameManager.Instance.mouseItemSlot.itemSlot.empty)
                {
                    UIManager.Instance.playerInventory.AddInventoryItem(itemSlot[4].itemCode, itemSlot[4].itemNum);
                }
                --itemSlot[0];
                --itemSlot[1];
                --itemSlot[2];
                --itemSlot[3];
                SetResipe();
            }
            return;
        }

        SetResipe();
    }
    public override void RightClickSlot(int slotNum)
    {
        RightClickSlotWork(slotNum);
        SetResipe();
    }

    public void IntSwap(ref int a, ref int b)
    {
        int tempInt = a;
        a = b;
        b = tempInt;
    }
}
