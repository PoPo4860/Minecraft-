using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    public int itemCode = 0;
    public int itemNum = 0;
    public ItemSlot(int _itemCode = 0, int _itemNum = 0)
    {
        itemCode = _itemCode;
        itemNum = _itemNum;
    }

    public bool empty
    {
        get { return (0 == itemCode && 0 == itemNum); }
    }
    public bool isMax 
    { 
        get { return (64 == itemNum); } 
    }

    public void Clear()
    {
        itemCode = 0;
        itemNum = 0;
    }

    // ��� �����ڴ� ������ ������ �������� ���մϴ�.
    // ������ ������ 0�� �̸��̸� 0���� ��ȯ, 64�� �̻��̸� 64�� ��ȯ�մϴ�.
    public static ItemSlot operator +(ItemSlot a) => a;
    public static ItemSlot operator -(ItemSlot a) => new ItemSlot(a.itemCode, -a.itemNum);
    public static ItemSlot operator +(ItemSlot a, ItemSlot b)
    {
        if(a.itemCode != b.itemCode)
        {
            Debug.LogWarning("�߸��� ������ ���� ���ϱ� ����");
            return new ItemSlot(0, 0);
        }
        int newItemNum = (a.itemNum + b.itemNum) >= 64 ? 64 : a.itemNum + b.itemNum;
        return new ItemSlot(a.itemCode, newItemNum);
    }
    public static ItemSlot operator +(ItemSlot a, int itemNum)
    {
        int newItemNum = (a.itemNum + itemNum) >= 64 ? 64 : a.itemNum + itemNum;
        return new ItemSlot(a.itemCode, newItemNum);
    }
    public static ItemSlot operator -(ItemSlot a, ItemSlot b)
    {
        if (a.itemCode != b.itemCode)
        {
            Debug.LogWarning("�߸��� ������ ���� ���� ����");
            return new ItemSlot(0, 0);
        }

        if (a.itemNum - b.itemNum < 0)
            return new ItemSlot(0, 0);
        return a + (-b);
    }
    public static ItemSlot operator -(ItemSlot a, int itemNum)
    {
        if (a.itemNum - itemNum < 0)
            return new ItemSlot(0, 0);
        return a + (-itemNum);
    }
    public static ItemSlot operator ++(ItemSlot a)
    {
        if(64 != a.itemNum && 0 != a.itemCode)
            ++a.itemNum;
        return a;
    }
    public static ItemSlot operator --(ItemSlot a)
    {
        if (0 != a.itemNum && 0 != a.itemCode)
            --a.itemNum;

        if (0 == a.itemNum)
            a.itemCode = 0;

        return a;
    }

    // �� �����ڴ� ������ �ڵ带 �������� ���մϴ�.
    public static bool operator ==(ItemSlot a, ItemSlot b)
    {
        return a.itemCode == b.itemCode;
    }
    public static bool operator !=(ItemSlot a, ItemSlot b)
    {
        return a.itemCode != b.itemCode;
    }
   
    public override bool Equals(object obj)
    {
        return obj is ItemSlot slot &&
               itemCode == slot.itemCode &&
               itemNum == slot.itemNum;
    }
    public override int GetHashCode()
    {
        int hashCode = -1140059519;
        hashCode = hashCode * -1521134295 + itemCode.GetHashCode();
        hashCode = hashCode * -1521134295 + itemNum.GetHashCode();
        return hashCode;
    }
}
