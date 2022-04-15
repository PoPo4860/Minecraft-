using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseItemSlot : MonoBehaviour
{
    public RectTransform mouseSlotTransform;
    public Image mouseSlotImage;
    public Text mouseSlotText;
    public ItemSlot ItemSlot;
    public static MouseItemSlot instance;
    private Vector3 mousePoint;

    public static MouseItemSlot Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    public ItemSlot itemSlot
    {
        get { return ItemSlot; }
        set { ItemSlot = value; }
    }

    private void Awake()
    {
        if (null == instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        mousePoint = Input.mousePosition;
        mousePoint.x -= (Screen.width / 2);
        mousePoint.y -= (Screen.height / 2);
        mouseSlotTransform.anchoredPosition = mousePoint;
    }
    public void ClearItemSlot()
    {
        itemSlot.Clear();
    }
    public void SwapItemSlot(ref ItemSlot _itemSlot)
    {
        int tempCode = itemSlot.itemCode;
        int tempNum = ItemSlot.itemNum;
        itemSlot.itemCode = _itemSlot.itemCode;
        ItemSlot.itemNum = _itemSlot.itemNum;
        _itemSlot.itemCode = tempCode;
        _itemSlot.itemNum = tempNum;

    }

    private void OnDisable()
    {
        int itemCode = ItemSlot.itemCode;
        int itemNum = ItemSlot.itemNum;
        if (0 != itemCode && 0 != itemNum)
        {
            PlayerInventory.Instance.AddInventoryItem(itemCode, itemNum);
            ItemSlot.itemCode = 0;
            ItemSlot.itemNum = 0;
        }

    }
}
