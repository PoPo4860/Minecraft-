using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    public ItemSlot(int _itemCode = 0, int _itemNum = 0)
    {
        itemCode = _itemCode;
        itemNum = _itemNum;
    }
    public int itemCode = 0;
    public int itemNum = 0;
}
