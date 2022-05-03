using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : ItemSlotUI
{
    private readonly int resipeResult = 9;
    private int[] resipe = new int[9];
    // [0][1][2]
    // [3][4][5]
    // [6][7][8]
    private void OnDisable()
    {
        for (int i = 0; i < 9; ++i)
        {
            if (false == itemSlot[i].empty)
            {
                UIManager.Instance.playerInventory.AddInventoryItem(itemSlot[i].itemCode, itemSlot[i].itemNum, false);
                itemSlot[i].Clear();
            }
        }
        itemSlot[9].Clear();
        for (int i = 0; i < 9; ++i)
            resipe[i] = 0;
    }
    private void SetResipe()
    {
        for(int i = 0; i < 9; ++i)
            resipe[i] = itemSlot[i].itemCode;

        for(int i = 0; i < 2; ++i)
        {
            if (0 == resipe[0] && 0 == resipe[1] && 0 == resipe[2])
            {
                Utile.IntSwap(ref resipe[0], ref resipe[3]);
                Utile.IntSwap(ref resipe[1], ref resipe[4]);
                Utile.IntSwap(ref resipe[2], ref resipe[5]);

                Utile.IntSwap(ref resipe[3], ref resipe[6]);
                Utile.IntSwap(ref resipe[4], ref resipe[7]);
                Utile.IntSwap(ref resipe[5], ref resipe[8]);
            }

            if (0 == resipe[0] && 0 == resipe[3] && 0 == resipe[6])
            {
                Utile.IntSwap(ref resipe[0], ref resipe[1]);
                Utile.IntSwap(ref resipe[3], ref resipe[4]);
                Utile.IntSwap(ref resipe[6], ref resipe[7]);

                Utile.IntSwap(ref resipe[1], ref resipe[2]);
                Utile.IntSwap(ref resipe[4], ref resipe[5]);
                Utile.IntSwap(ref resipe[7], ref resipe[8]);
            }

        }

        ResipeResult result = CraftingResipe.GetResipe(resipe);
        itemSlot[9].itemCode = result.itemCode;
        itemSlot[9].itemNum = (result.itemCode == 0) ? 0 : result.itemNum;
    }
    public override void LeftClickSlot(int slotNum)
    {
        if (resipeResult != slotNum)
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
                    for (int i = 0; i < 9; ++i)
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
                        for (int i = 0; i < 9; ++i)
                            --itemSlot[i];
                    }
                }
            }
        }
        SetResipe();
    }
    public override void LeftShiftClickSlot(int slotNum)
    {
        if (resipeResult != slotNum)
        {
            LeftShiftClickSlotWork(slotNum);
        }
        else
        {
            while (false == itemSlot[resipeResult].empty)
            {
                UIManager.Instance.playerInventory.AddInventoryItem(itemSlot[resipeResult].itemCode, itemSlot[resipeResult].itemNum);

                for (int i = 0; i < 9; ++i)
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
