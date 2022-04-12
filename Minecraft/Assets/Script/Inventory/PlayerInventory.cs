using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private ItemSlot[] itemSlot = new ItemSlot[4*9];

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


    private static PlayerInventory instance;
    private void Awake()
    {
        if (null == instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static PlayerInventory Instance
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

    void Start()
    {
        for (int i = 0; i < 36; ++i)
        {
            itemSlot[i] = new ItemSlot();
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
        //int x = slotNum % 9;
        //int y = slotNum / 9;
        moustItemSlot.SwapItemSlot(ref itemSlot[slotNum]);
        SetItemSlotImage(slotNum);
    }
    private void SetItemSlotImage(int slotNum)
    {
        //int num = 0;
        //for (int y = 0; y < itemSlotPos.y; ++y) 
        //    num += 9;
        //num += itemSlotPos.x;

        int itemCode = itemSlot[slotNum].itemCode;
        string itemName = CodeData.GetBlockInfo(itemCode).blockName;
        itemSlotTexture[slotNum].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);

        int itemNum = itemSlot[slotNum].itemNum;
        itemSlotText[slotNum].text = 0 == itemNum ? "" : $"{itemNum}";

        // 퀵슬롯 설정
        if (27 <= slotNum) 
        {
            itemQuickSlotTexture[slotNum - 27].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);
            itemQuickSlotText[slotNum - 27].text = 0 == itemNum ? "" : $"{itemNum}";
        }
    }
    public void AddInventoryItem(in int itemCode, in int itemNum)
    {
        if(true == CheckCanAddInventory(itemCode, out int itemSlotNum))
        {
            if(CodeData.BLOCK_AIR == itemSlot[itemSlotNum].itemCode)
            {   // 해당 위치가 비어있다면
                itemSlot[itemSlotNum].itemCode = itemCode;
                itemSlot[itemSlotNum].itemNum = itemNum;
                SetInventoryItem(itemSlot[itemSlotNum], itemCode);
            }
            else
            {   // 해당 위치에 아이템이 있다면
                itemSlot[itemSlotNum].itemNum += itemNum;
                SetInventoryItem(itemSlot[itemSlotNum], itemCode);
            }
            SetItemSlotImage(itemSlotNum);
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
    public bool CheckCanAddInventory(in int itemCode, out int itemSlotNum)
    {
        itemSlotNum = -1;
        for (int i = 0; i < 36; ++i)
        {
            ++itemSlotNum;

            if (itemCode == itemSlot[itemSlotNum].itemCode && itemSlot[itemSlotNum].itemNum < slotInMaxItem)
                return true;
        }

        itemSlotNum = -1;
        for (int i = 0; i < 36; ++i)
        {
            ++itemSlotNum;
            if (0 == itemSlot[itemSlotNum].itemCode)
                return true;
        }

        return false;
    }

    public ItemSlot GetQuickItemSlot(int slotNum)
    {
        return itemSlot[slotNum + 27];
    }
    public ItemSlot RightClickQuickSlotItem(int slotNum)
    {
        slotNum += 27;
        int bufferItemCode = itemSlot[slotNum].itemCode;
        int bufferItemNum = itemSlot[slotNum].itemNum;

        if(0 != bufferItemNum)
            --itemSlot[slotNum].itemNum;

        if(0 == itemSlot[slotNum].itemNum)
            itemSlot[slotNum].itemCode = 0;
        
        SetItemSlotImage(slotNum);
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
