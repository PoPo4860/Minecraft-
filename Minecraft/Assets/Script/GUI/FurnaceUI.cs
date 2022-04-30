using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceUI : ItemSlotUI
{
    private readonly int rawMaterialSlotNum = 0;
    private readonly int firewoodSlotNum = 1;
    private readonly int resultSlotNum = 2;

    private float combustionTime;
    private readonly float combustionMaxTime = 10;
    private float combustionPower;

    //
    // 연소 시간이 10이 되면 굽기 성공
    // 연소 파워가 0 이상이라면 연소시간에 추가
    // 연소 파워가 0 이하라면 연소시간 감소
    // 아이템마다 연소파워가 다름
    //
    public override void LeftClickSlot(int slotNum)
    {
        if (resultSlotNum == slotNum)
        {
            ResultSlotNumWork();
        }

        if (rawMaterialSlotNum == slotNum)
        {
            LeftClickSlotWork(slotNum);
        }

        if (firewoodSlotNum == slotNum)
        {
            LeftClickSlotWork(slotNum);
        }
    }

    private void Check()
    {
        if(fireWoodCheck() && RawMaterialCheck())
        {   // 연소가 가능하다면
            combustionTime = combustionPower;
        }
    }
    private bool fireWoodCheck()
    {
        return false;
    }
    private bool RawMaterialCheck()
    {
        return false;
    }
    public override void RightClickSlot(int slotNum)
    {
        if (resultSlotNum != slotNum)
            RightClickSlotWork(slotNum);
    }
    private void ResultSlotNumWork()
    {
        ItemSlot mouseSlot = GameManager.Instance.mouseItemSlot.itemSlot;
        if (false == itemSlot[resultSlotNum].empty)
        {// 

            if (true == mouseSlot.empty)
            {
                GameManager.Instance.mouseItemSlot.SwapItemSlot(ref itemSlot[resultSlotNum]);
            }
            else
            {
                int slotItemNum = itemSlot[resultSlotNum].itemNum;
                int mouseItemNum = 64 - mouseSlot.itemNum;
                if (mouseItemNum >= slotItemNum && mouseSlot == itemSlot[resultSlotNum])
                {
                    mouseSlot += slotItemNum;
                }
            }
        }
    }
}
