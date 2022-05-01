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

    // 연소 파워 추가 조건
    // 1. 원료와 떌감칸에 알맞은 아이템이 들어가 있다면 알맞은 파워 증가
    // 2. 결과창에 연소결과 아이템이 추가 될 수 있다면.
    // 
    // 연소 시간 추가 조건
    // 1. 연소 파워가 0이 상일 때에 파워를 줄이고 시간을 추가
    // 2. 연소 파워가 0이라면 시간을 줄임.
    // 3. 연소시간이 10이라면 0으로 초기화
    // 4. 결과창에 연소결과 아이템이 추가 될 수 있다면.
    // 
    // 연소 조건
    // 1. 결과창에 연소결과 아이템이 추가 될 수 있다면.
    // 2. 연소시간이 10이 채워졌을 때에
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
