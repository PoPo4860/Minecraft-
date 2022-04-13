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
    private void Awake()
    {
        if (null == instance)
            instance = this;
        else
            Destroy(gameObject);
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
    private void SetItemSlotImage(int slotNum)
    {
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
    public void LeftClickSlot(int slotNum)
    {
        if (true == moustItemSlot.itemSlot.empty && true == itemSlot[slotNum].empty)
            return;

        // 슬롯간 교환이 일어나야 할때
        if (true == moustItemSlot.itemSlot.empty ||
            true == itemSlot[slotNum].empty ||
            itemSlot[slotNum].isMax ||
            itemSlot[slotNum] != moustItemSlot.itemSlot)
            moustItemSlot.SwapItemSlot(ref itemSlot[slotNum]);

        // 슬롯에 아이템을 추가해야 할때
        if(itemSlot[slotNum] == moustItemSlot.itemSlot)
        {
            int canAddItemNum = 64 - itemSlot[slotNum].itemNum;
            if (moustItemSlot.itemSlot.itemNum <= canAddItemNum)
            {
                itemSlot[slotNum] += moustItemSlot.itemSlot;
                moustItemSlot.itemSlot.Clear();
            }
            else
            {
                itemSlot[slotNum].itemNum = 64;
                moustItemSlot.itemSlot -= canAddItemNum;
            }
        }
        SetItemSlotImage(slotNum);
        moustItemSlot.SetItemSlotImage();
    }

    public void RightClickSlot(int slotNum)
    {
        if (true == moustItemSlot.itemSlot.empty)
            return;

        if (true == itemSlot[slotNum].empty)
        {
            itemSlot[slotNum].itemCode = moustItemSlot.itemSlot.itemCode;
            ++itemSlot[slotNum];
            --moustItemSlot.itemSlot;
        }

        else if (false == itemSlot[slotNum].isMax && moustItemSlot.itemSlot == itemSlot[slotNum])
        {
            ++itemSlot[slotNum];
            --moustItemSlot.itemSlot;
        }



        SetItemSlotImage(slotNum);
        moustItemSlot.SetItemSlotImage();
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

        if(false == itemSlot[slotNum].empty)
            --itemSlot[slotNum];
        
        SetItemSlotImage(slotNum);
        if(0 != bufferItemNum)
            return new ItemSlot(bufferItemCode, bufferItemNum);
        else
            return new ItemSlot(0, bufferItemNum);
    }

}
