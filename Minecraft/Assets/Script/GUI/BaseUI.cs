using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : ItemSlotUI
{
    public enum EBase { Base, Empty}
    public EBase tpye;
    public override void LeftClickSlot(int slotNum)
    {
        if(tpye == EBase.Empty)
            GameManager.Instance.mouseItemSlot.DropItemFromInventoy(64);
    }
    public override void LeftShiftClickSlot(int slotNum)
    {
    }
    public override void RightClickSlot(int slotNum)
    {
        if (tpye == EBase.Empty)
            GameManager.Instance.mouseItemSlot.DropItemFromInventoy(1);
    }
}
