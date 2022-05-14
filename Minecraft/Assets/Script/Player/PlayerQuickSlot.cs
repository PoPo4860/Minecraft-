using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQuickSlot : MonoBehaviour
{
    [SerializeField] private RectTransform selectQuickSlotRect;
    [SerializeField] private Text selectQuickSlotText;
    private int selectQuickSlotItemNum = 0;
    [HideInInspector] public int currentSelectNum = 0;
    private int getKeyNum = 0;

    public PlayerRightHand playerRightHand;
    public TargetBlock targetBlock;
    public ItemSlot[] itemSlot;

    private void Start()
    {
        SetQuickSlot();
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        getKeyNum = GetItemQuickSlotNumFromInput();
        if (-1 != getKeyNum || selectQuickSlotItemNum != itemSlot[currentSelectNum].itemNum)
        {
            if (-1 != getKeyNum) 
                currentSelectNum = getKeyNum;
            SetQuickSlot();
        }
    }

    private void SetQuickSlot()
    {
        selectQuickSlotItemNum = itemSlot[currentSelectNum].itemNum;
        selectQuickSlotRect.position = itemSlot[currentSelectNum].itemImage.transform.position;
        int itemCode = itemSlot[currentSelectNum].itemCode;
        playerRightHand.SetItemRender(itemCode);
        if (CodeData.BLOCK_Air != itemCode)
            selectQuickSlotText.text = CodeData.GetCodeName(itemCode);
        else
            selectQuickSlotText.text = "";
        ItemType itemType = CodeData.GetItemInfo(itemSlot[currentSelectNum].itemCode);
        targetBlock.SetItemType(itemType);
    }

    public ushort UseQuickSlotItemCode()
    {
        int itemCode = UIManager.Instance.playerInventory.RightClickQuickSlotItem(currentSelectNum);
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
