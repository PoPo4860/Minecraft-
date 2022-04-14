using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQuickSlot : MonoBehaviour
{
    public RectTransform[] quickSlot;
    public RectTransform selectQuickSlotRect;
    public Text selectQuickSlotText;

    private int currentSelectNum = 0;
    private void Start()
    {
        selectQuickSlotRect.position = quickSlot[0].position;
        selectQuickSlotText.text = "";
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (-1 != GetItemQuickSlotNumFromInput())
        {
            currentSelectNum = GetItemQuickSlotNumFromInput();
            selectQuickSlotRect.position = quickSlot[currentSelectNum].position;
            int itemCode = PlayerInventory.Instance.GetQuickItemSlot(currentSelectNum).itemCode;
            if (CodeData.BLOCK_AIR != itemCode)
                selectQuickSlotText.text = CodeData.GetBlockInfo(itemCode).blockName;
            else
                selectQuickSlotText.text = "";
        }
    }

    public ushort UseQuickSlotItemCode()
    {
        return (ushort)PlayerInventory.Instance.RightClickQuickSlotItem(currentSelectNum).itemCode;
    }
    private int GetItemQuickSlotNumFromInput()
    {
        for (int i = 0; i < 9; ++i) 
            if (Input.GetKeyDown(KeyCode.None + (49 + i))) 
                return i;
        return -1;
    }
}
