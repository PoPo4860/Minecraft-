using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseItemSlot : MonoBehaviour
{
    public RectTransform mouseSlotTransform;
    public Image mouseSlotImage;
    public Text mouseSlotText;
    private ItemSlot mouseItemSlot = new ItemSlot();
    public ItemSlot itemSlot
    {
        get { return mouseItemSlot; }
        set { mouseItemSlot = value; }
    }
    

    private Vector3 mousePoint;
    void Start()
    {
    }
    void Update()
    {
        mousePoint = Input.mousePosition;
        mousePoint.x -= (Screen.width / 2);
        mousePoint.y -= (Screen.height / 2);
        mouseSlotTransform.anchoredPosition = mousePoint;
    }

    public void SwapItemSlot(ref ItemSlot itemSlot)
    {
        ItemSlot temp = itemSlot;
        itemSlot = mouseItemSlot;
        mouseItemSlot = temp;
    }

    public void SetItemSlotImage()
    {
        int itemCode = mouseItemSlot.itemCode;
        string itemName = CodeData.GetBlockInfo(itemCode).blockName;
        mouseSlotImage.sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);

        int itemNum = mouseItemSlot.itemNum;
        mouseSlotText.text = 0 == itemNum ? "" : $"{itemNum}";
    }

    private void OnEnable()
    {
        SetItemSlotImage();
    }
    private void OnDisable()
    {
        int itemCode = mouseItemSlot.itemCode;
        int itemNum = mouseItemSlot.itemNum;
        if (0 != itemCode && 0 != itemNum)
        {
            PlayerInventory.Instance.AddInventoryItem(itemCode, itemNum);
            mouseItemSlot.itemCode = 0;
            mouseItemSlot.itemNum = 0;
        }

    }
}
