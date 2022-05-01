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

    // ���� �Ŀ� �߰� ����
    // 1. ����� ����ĭ�� �˸��� �������� �� �ִٸ� �˸��� �Ŀ� ����
    // 2. ���â�� ���Ұ�� �������� �߰� �� �� �ִٸ�.
    // 
    // ���� �ð� �߰� ����
    // 1. ���� �Ŀ��� 0�� ���� ���� �Ŀ��� ���̰� �ð��� �߰�
    // 2. ���� �Ŀ��� 0�̶�� �ð��� ����.
    // 3. ���ҽð��� 10�̶�� 0���� �ʱ�ȭ
    // 4. ���â�� ���Ұ�� �������� �߰� �� �� �ִٸ�.
    // 
    // ���� ����
    // 1. ���â�� ���Ұ�� �������� �߰� �� �� �ִٸ�.
    // 2. ���ҽð��� 10�� ä������ ����
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
        {   // ���Ұ� �����ϴٸ�
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
