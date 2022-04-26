using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseItemSlot : MonoBehaviour
{
    public RectTransform mouseSlotTransform;
    public Image mouseSlotImage;
    public Text mouseSlotText;
    public ItemSlot _itemSlot;
    private Vector3 mousePoint;

    public ItemSlot itemSlot
    {
        get { return _itemSlot; }
        set { _itemSlot = value; }
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
        int tempNum = this._itemSlot.itemNum;
        itemSlot.itemCode = _itemSlot.itemCode;
        this._itemSlot.itemNum = _itemSlot.itemNum;
        _itemSlot.itemCode = tempCode;
        _itemSlot.itemNum = tempNum;
    }

    public void DropItemFromInventoy(int dropItemNum)
    {
        if (true == itemSlot.empty)
            return;

        int itemCode = itemSlot.itemCode;
        if (itemSlot.itemNum < dropItemNum)
            dropItemNum = itemSlot.itemNum;

        itemSlot -= dropItemNum;
        Vector3 vec = GameManager.Instance.player.cameraTransform.position;
        Vector3 vec2 = GameManager.Instance.player.cameraTransform.forward / 1.5f;
        GameManager.Instance.itemManager.AddDropItem(itemCode, dropItemNum, vec, vec2);
    }

    private void OnDisable()
    {
        int itemCode = _itemSlot.itemCode;
        int itemNum = _itemSlot.itemNum;
        if (0 != itemCode && 0 != itemNum)
        {
            UIManager.Instance.playerInventory.AddInventoryItem(itemCode, itemNum);
            _itemSlot.itemCode = 0;
            _itemSlot.itemNum = 0;
        }

    }
}
