using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQuickSlot : MonoBehaviour
{
    public RectTransform selectQuickSlotRect;
    public Text selectQuickSlotText;

    public ItemSlot[] itemSlot;

    private int currentSelectNum = 0;
    private void Start()
    {
        //selectQuickSlotRect.position = itemSlot[0].itemImage.transform.position;
        //selectQuickSlotText.text = "";
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (-1 != GetItemQuickSlotNumFromInput())
        {
            currentSelectNum = GetItemQuickSlotNumFromInput();
            selectQuickSlotRect.position = itemSlot[currentSelectNum].itemImage.transform.position;
            int itemCode = itemSlot[currentSelectNum].itemCode;
            if (CodeData.BLOCK_AIR != itemCode)
                selectQuickSlotText.text = CodeData.GetBlockInfo(itemCode).blockName;
            else
                selectQuickSlotText.text = "";
        }
    }

    public ushort UseQuickSlotItemCode()
    {
        GameManager.Instance.playerInventory.RightClickQuickSlotItem(currentSelectNum, out int itemCode, out int itemNum);
        return (ushort)itemCode;
    }
    private int GetItemQuickSlotNumFromInput()
    {
        for (int i = 0; i < 9; ++i) 
            if (Input.GetKeyDown(KeyCode.None + (49 + i))) 
                return i;
        return -1;
    }
}
