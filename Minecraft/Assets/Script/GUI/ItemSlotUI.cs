using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] public ItemSlot[] itemSlot;
    // Start is called before the first frame update
    public void LeftClickSlotWork(int slotNum)
    {
        if (true == GameManager.Instance.mouseItemSlot.itemSlot.empty &&
            true == itemSlot[slotNum].empty)
            return;

         // 슬롯간 교환이 일어나야 할때
         if (true == GameManager.Instance.mouseItemSlot.itemSlot.empty ||
             true == GameManager.Instance.mouseItemSlot.itemSlot.isMax ||
             true == itemSlot[slotNum].empty ||
             true == itemSlot[slotNum].isMax ||
             itemSlot[slotNum] != GameManager.Instance.mouseItemSlot.itemSlot)
             GameManager.Instance.mouseItemSlot.SwapItemSlot(ref itemSlot[slotNum]);

         // 슬롯에 아이템을 추가해야 할때
         if (itemSlot[slotNum] == GameManager.Instance.mouseItemSlot.itemSlot)
         {
             int canAddItemNum = 64 - itemSlot[slotNum].itemNum;
             if (GameManager.Instance.mouseItemSlot.itemSlot.itemNum <= canAddItemNum)
             {
                 itemSlot[slotNum] += GameManager.Instance.mouseItemSlot.itemSlot;
                 GameManager.Instance.mouseItemSlot.ClearItemSlot();
             }
             else
             {
                 itemSlot[slotNum].itemNum = 64;
                 GameManager.Instance.mouseItemSlot.itemSlot -= canAddItemNum;
             }
         }
    }
    public void LeftShiftClickSlotWork(int slotNum)
    {
        int itemCode = itemSlot[slotNum].itemCode;
        int itemNum = itemSlot[slotNum].itemNum;
        itemSlot[slotNum].Clear();
        if (slotNum < 27)
            UIManager.Instance.playerInventory.AddInventoryItem(itemCode, itemNum, true);
        else
            UIManager.Instance.playerInventory.AddInventoryItem(itemCode, itemNum, false);
    }
    public void RightClickSlotWork(int slotNum)
    {
        if (true == GameManager.Instance.mouseItemSlot.itemSlot.empty &&
            1 != itemSlot[slotNum].itemNum)
        {
            int temp = (1 == itemSlot[slotNum].itemNum % 2) ? 1 : 0;
            int itemNum = itemSlot[slotNum].itemNum /= 2;
            itemSlot[slotNum].itemNum = itemNum + temp;
            if (0 != itemNum)
                GameManager.Instance.mouseItemSlot.itemSlot.itemCode = itemSlot[slotNum].itemCode;

            GameManager.Instance.mouseItemSlot.itemSlot.itemNum = itemNum;
        }
        else
        {
            if (true == itemSlot[slotNum].empty)
            {
                itemSlot[slotNum].itemCode = GameManager.Instance.mouseItemSlot.itemSlot.itemCode;
                ++itemSlot[slotNum];
                --GameManager.Instance.mouseItemSlot.itemSlot;
            }

            else if (false == itemSlot[slotNum].isMax && GameManager.Instance.mouseItemSlot.itemSlot == itemSlot[slotNum])
            {
                ++itemSlot[slotNum];
                --GameManager.Instance.mouseItemSlot.itemSlot;
            }
        }
    }


    public virtual void LeftClickSlot(int slotNum)
    {
        LeftClickSlotWork(slotNum);
    }
    public virtual void LeftShiftClickSlot(int slotNum)
    {
        LeftShiftClickSlotWork(slotNum);
    }
    public virtual void RightClickSlot(int slotNum)
    {
        RightClickSlotWork(slotNum);
    }
}
