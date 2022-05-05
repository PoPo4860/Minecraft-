using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCraftingTable : ItemSlotUI
{
    private readonly int resipeResult = 4;
    private int[] resipe = new int[9];
    // [0][1][2]
    // [3][4][5]
    // [6][7][8]
    private void OnDisable()
    {
        for(int i = 0; i < 4; ++i)
        {
            if(false == itemSlot[i].empty)
            {
                UIManager.Instance.playerInventory.AddInventoryItem(itemSlot[i].itemCode, itemSlot[i].itemNum, false);
                itemSlot[i].Clear();
            }
        }
        itemSlot[4].Clear();
        for (int i = 0; i < 9; ++i)
            resipe[i] = 0;
    }


    private void SetResipe()
    {
        resipe[0] = itemSlot[0].itemCode;
        resipe[1] = itemSlot[1].itemCode;
        resipe[3] = itemSlot[2].itemCode;
        resipe[4] = itemSlot[3].itemCode;
        
        if (0 == resipe[0] && 0 == resipe[1])
        {
            Utile.IntSwap(ref resipe[0],ref resipe[3]);
            Utile.IntSwap(ref resipe[1],ref resipe[4]);
        }

        if (0 == resipe[0] && 0 == resipe[3])
        {
            Utile.IntSwap(ref resipe[0], ref resipe[1]);
            Utile.IntSwap(ref resipe[3], ref resipe[4]);
        }

        ResipeResult result = CraftingResipe.GetResipe(resipe);
        itemSlot[4].itemCode = result.itemCode;
        itemSlot[4].itemNum = (result.itemCode == 0) ? 0 : result.itemNum;
    }
    public override void LeftClickSlot(int slotNum)
    {
        if(4 != slotNum)
        {
            LeftClickSlotWork(slotNum);
        }
        else
        {
            ItemSlot mouseSlot = GameManager.Instance.mouseItemSlot.itemSlot;
            if (false == itemSlot[resipeResult].empty)
            {// 

                if (true == mouseSlot.empty)
                {
                    GameManager.Instance.mouseItemSlot.SwapItemSlot(ref itemSlot[resipeResult]);
                    for (int i = 0; i < 4; ++i)
                        --itemSlot[i];
                }
                else
                {
                    int slotItemNum = itemSlot[resipeResult].itemNum;
                    int mouseItemNum = 64 - mouseSlot.itemNum;
                    if (mouseItemNum >= slotItemNum &&
                        mouseSlot == itemSlot[resipeResult])
                    {
                        mouseSlot += slotItemNum;
                        for (int i = 0; i < 4; ++i)
                            --itemSlot[i];
                    }
                }
            }
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
            while(false == itemSlot[resipeResult].empty)
            {
                UIManager.Instance.playerInventory.AddInventoryItem(itemSlot[resipeResult].itemCode, itemSlot[resipeResult].itemNum);
             
                for(int i = 0; i < 4; ++i)
                    --itemSlot[i];

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
}
