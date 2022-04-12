using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQuickSlot : MonoBehaviour
{
    public RectTransform selectQuickSlotRect;
    public Text selectQuickSlotText;
    public RectTransform[] quickSlot;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
            return 0;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            return 1;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            return 2;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            return 3;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            return 4;

        if (Input.GetKeyDown(KeyCode.Alpha6))
            return 5;

        if (Input.GetKeyDown(KeyCode.Alpha7))
            return 6;

        if (Input.GetKeyDown(KeyCode.Alpha8))
            return 7;

        if (Input.GetKeyDown(KeyCode.Alpha9))
            return 8;
        return -1;
    }
}
