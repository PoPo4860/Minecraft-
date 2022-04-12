using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private ItemSlot[,] itemSlot = new ItemSlot[4, 9];

    [Header("인벤토리 아이템 슬롯")]
    public Image[] itemSlotTexture;
    public Text[] itemSlotText;

    [Header("아이템 퀵 슬롯")]
    public Image[] itemQuickSlotTexture;
    public Text[] itemQuickSlotText;
    public PlayerQuickSlot playerQuickSlot;

    [Header("마우스 아이템 슬롯")]
    public MouseItemSlot moustItemSlot;

    private const int slotInMaxItem = 64;
    
    
    void Start()
    {
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                itemSlot[i,j] = new ItemSlot();
            }
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            AddInventoryItem(CodeData.BLOCK_BEDROCK, 10);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AddInventoryItem(CodeData.BLOCK_COAL, 10);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            AddInventoryItem(CodeData.BLOCK_GRASS, 10);
    }
    public void ClickSlot(int slotNum)
    {
        int x = slotNum % 9;
        int y = slotNum / 9;
        moustItemSlot.SwapItemSlot(ref itemSlot[y, x]);
        SetItemSlotImage(new Vector2Int(x, y));
    }
    private void SetItemSlotImage(in Vector2Int itemSlotPos)
    {
        int num = 0;
        for (int y = 0; y < itemSlotPos.y; ++y) 
            num += 9;
        num += itemSlotPos.x;

        int itemCode = itemSlot[itemSlotPos.y, itemSlotPos.x].itemCode;
        string itemName = CodeData.GetBlockInfo(itemCode).blockName;
        itemSlotTexture[num].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);

        int itemNum = itemSlot[itemSlotPos.y, itemSlotPos.x].itemNum;
        itemSlotText[num].text = 0 == itemNum ? "" : $"{itemNum}";

        // 퀵슬롯 설정
        if (3 == itemSlotPos.y) 
        {
            itemQuickSlotTexture[itemSlotPos.x].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);
            itemQuickSlotText[itemSlotPos.x].text = 0 == itemNum ? "" : $"{itemNum}";
        }
    }
    public void AddInventoryItem(in int itemCode, in int itemNum)
    {
        if(true == CheckCanAddInventory(itemCode, out Vector2Int itemSlotPos))
        {
            if(CodeData.BLOCK_AIR == itemSlot[itemSlotPos.y,itemSlotPos.x].itemCode)
            {   // 해당 위치가 비어있다면
                itemSlot[itemSlotPos.y, itemSlotPos.x].itemCode = itemCode;
                itemSlot[itemSlotPos.y, itemSlotPos.x].itemNum = itemNum;
                SetInventoryItem(itemSlot[itemSlotPos.y, itemSlotPos.x], itemCode);
            }
            else
            {   // 해당 위치에 아이템이 있다면
                itemSlot[itemSlotPos.y, itemSlotPos.x].itemNum += itemNum;
                SetInventoryItem(itemSlot[itemSlotPos.y, itemSlotPos.x], itemCode);
            }
            SetItemSlotImage(itemSlotPos);
        }
    }
    private void SetInventoryItem(ItemSlot itemSlot, int itemCode)
    {
        int remainItem = itemSlot.itemNum - 64;
        if (0 < remainItem)
        {
            itemSlot.itemNum = 64;
            AddInventoryItem(itemCode, remainItem);
        }
    }
    public bool CheckCanAddInventory(in int itemCode, out Vector2Int itemSlotPos)
    {
        itemSlotPos = new Vector2Int(-1, -1);
        for (int i = 0; i < 4; ++i)
        {
            ++itemSlotPos.y;
            itemSlotPos.x = -1;
            for (int j = 0; j < 9; ++j)
            {
                ++itemSlotPos.x;
                if (itemCode == itemSlot[i, j].itemCode && itemSlot[i, j].itemNum < slotInMaxItem)
                    return true;
            }
        }

        itemSlotPos = new Vector2Int(-1, -1);
        for (int i = 0; i < 4; ++i)
        {
            ++itemSlotPos.y;
            itemSlotPos.x = -1;
            for (int j = 0; j < 9; ++j)
            {
                ++itemSlotPos.x;
                if (0 == itemSlot[i, j].itemCode)
                    return true;
            }
        }

        return false;
    }

    public ItemSlot GetQuickItemSlot(int slotNum)
    {
        return itemSlot[3, slotNum];
    }
    public ItemSlot RightClickQuickSlotItem(int slotNum)
    {
        int bufferItemCode = itemSlot[3, slotNum].itemCode;
        int bufferItemNum = itemSlot[3, slotNum].itemNum;

        if(0 != bufferItemNum)
            --itemSlot[3, slotNum].itemNum;

        if(0 == itemSlot[3, slotNum].itemNum)
            itemSlot[3, slotNum].itemCode = 0;
        
        SetItemSlotImage(new Vector2Int(slotNum, 3));
        if(0 != bufferItemNum)
            return new ItemSlot(bufferItemCode, bufferItemNum);
        else
            return new ItemSlot(0, bufferItemNum);

    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
