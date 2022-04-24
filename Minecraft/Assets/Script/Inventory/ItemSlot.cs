using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private int _itemCode = 0;
    private int _itemNum = 0;

    public int itemNum
    {
        get { return _itemNum; }
        set
        {
            _itemNum = value;
            if (0 == itemNum)
                itemCode = 0;
            if (_itemNum < 0)
                _itemNum = 0;

            SetItemImage();
        }
    }
    public int itemCode
    {
        get { return _itemCode; }
        set { _itemCode = value; }
    }

    [HideInInspector] public Image itemImage;
    [HideInInspector] public Text itemText;
    
    private void Awake()
    {
        if (null == itemImage)
            itemImage = GetComponent<Image>();
        if (null == itemText)
            itemText = GetComponentInChildren<Text>();
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
    public void SetItemImage()
    {
        string itemName = CodeData.GetCodeName(itemCode);
        itemImage.sprite = Resources.Load<Sprite>("Icon/" + itemName);
        itemText.text = (0 == itemNum) ? "" : $"{itemNum}";
    }

    // 계산 연산자는 아이템 갯수를 기준으로 비교합니다.
    // 아이템 갯수가 0개 미만이면 0개를 반환, 64개 이상이면 64를 반환합니다.
    public static ItemSlot operator +(ItemSlot a) => a;
    public static ItemSlot operator -(ItemSlot a)
    {
        a.itemNum = -a.itemNum;
        return a;
    }
    public static ItemSlot operator +(ItemSlot a, ItemSlot b)
    {
        if(a.itemCode != b.itemCode)
        {
            Debug.LogWarning("잘못된 아이템 슬롯 더하기 연산");
            a.itemCode = 0;
            a.itemNum = 0;
            return a;
        }
        int newItemNum = (a.itemNum + b.itemNum) >= 64 ? 64 : a.itemNum + b.itemNum;
        a.itemNum = newItemNum;
        return a;
    }
    public static ItemSlot operator +(ItemSlot a, int itemNum)
    {
        int newItemNum = (a.itemNum + itemNum) >= 64 ? 64 : a.itemNum + itemNum;
        a.itemNum = newItemNum;
        return a;
    }
    public static ItemSlot operator -(ItemSlot a, ItemSlot b)
    {
        if (a.itemCode != b.itemCode)
        {
            Debug.LogWarning("잘못된 아이템 슬롯 빼기 연산");
            a.itemCode = 0;
            a.itemNum = 0;
            return a;
        }

        if (a.itemNum - b.itemNum < 0)
        {
            a.itemCode = 0;
            a.itemNum = 0;
            return a;
        }
        return a + (-b);
    }
    public static ItemSlot operator -(ItemSlot a, int itemNum)
    {
        if (a.itemNum - itemNum < 0)
        {
            a.itemCode = 0;
            a.itemNum = 0;
            return a;
        }
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

    // 비교 연산자는 아이템 코드를 기준으로 비교합니다.
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
