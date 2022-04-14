using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public ItemSlot[] itemSlot;

    [Header("인벤토리 아이템 슬롯")]
    public Image[] itemSlotTexture;
    public Text[] itemSlotText;

    [Header("아이템 퀵 슬롯")]
    public Image[] itemQuickSlotTexture;
    public Text[] itemQuickSlotText;
    public PlayerQuickSlot playerQuickSlot;

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

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddInventoryItem(CodeData.BLOCK_BEDROCK, 10);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AddInventoryItem(CodeData.BLOCK_COAL, 10);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            AddInventoryItem(CodeData.BLOCK_GRASS, 10);
    }
    private void SetItemSlotImage(int slotNum)
    {
        //int itemCode = itemSlot[slotNum].itemCode;
        //string itemName = CodeData.GetBlockInfo(itemCode).blockName;
        //itemSlotTexture[slotNum].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);

        //int itemNum = itemSlot[slotNum].itemNum;
        //itemSlotText[slotNum].text = 0 == itemNum ? "" : $"{itemNum}";

        //// 퀵슬롯 설정
        //if (27 <= slotNum)
        //{
        //    itemQuickSlotTexture[slotNum - 27].sprite = Resources.Load<Sprite>("BlockIcon/" + itemName);
        //    itemQuickSlotText[slotNum - 27].text = 0 == itemNum ? "" : $"{itemNum}";
        //}
    }
    private void SetInventoryItem(ItemSlot itemSlot, int itemCode, bool quickPriority = true)
    {
        int remainItem = itemSlot.itemNum - 64;
        if (0 < remainItem)
        {
            itemSlot.itemNum = 64;
            AddInventoryItem(itemCode, remainItem, quickPriority);
        }
    }

    public void AddInventoryItem(in int itemCode, in int itemNum, bool quickPriority = true)
    {
        if (true == CheckCanAddInventory(itemCode, out int itemSlotNum, quickPriority))
        {
            if (CodeData.BLOCK_AIR == itemSlot[itemSlotNum].itemCode)
            {   // 해당 위치가 비어있다면
                itemSlot[itemSlotNum].itemCode = itemCode;
                itemSlot[itemSlotNum].itemNum = itemNum;
                SetInventoryItem(itemSlot[itemSlotNum], itemCode, quickPriority);
            }
            else
            {   // 해당 위치에 아이템이 있다면
                itemSlot[itemSlotNum].itemNum += itemNum;
                SetInventoryItem(itemSlot[itemSlotNum], itemCode, quickPriority);
            }
            SetItemSlotImage(itemSlotNum);
        }
    }
    public bool CheckCanAddInventory(in int itemCode, out int itemSlotNum, bool quickPriority)
    {
        itemSlotNum = -1;
        for (int slotNum = 0; slotNum < 36; ++slotNum)
        {
            if (itemCode == itemSlot[slotNum].itemCode && itemSlot[slotNum].itemNum < slotInMaxItem)
            {
                itemSlotNum = slotNum;
                return true;
            }
        }

        if (false == quickPriority)
        {
            for (int slotNum = 0; slotNum < 36; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }
        }
        else
        {
            for (int slotNum = 27; slotNum < 36; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }

            for (int slotNum = 0; slotNum < 27; ++slotNum)
            {
                if (0 == itemSlot[slotNum].itemCode)
                {
                    itemSlotNum = slotNum;
                    return true;
                }
            }
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

        //if (false == itemSlot[slotNum].empty)
        //    --itemSlot[slotNum];

        SetItemSlotImage(slotNum);
            return itemSlot[slotNum]--;
    }

    public void LeftClickSlot(int slotNum)
    {
        if (true == MouseItemSlot.Instance.itemSlot.empty &&
            true == itemSlot[slotNum].empty)
            return;

        if (false == Input.GetKey(KeyCode.LeftShift))
        {
            // 슬롯간 교환이 일어나야 할때
            if (true == MouseItemSlot.Instance.itemSlot.empty ||
                true == MouseItemSlot.Instance.itemSlot.isMax ||
                true == itemSlot[slotNum].empty ||
                true == itemSlot[slotNum].isMax ||
                itemSlot[slotNum] != MouseItemSlot.Instance.itemSlot)
                MouseItemSlot.Instance.SwapItemSlot(ref itemSlot[slotNum]);

            // 슬롯에 아이템을 추가해야 할때
            if (itemSlot[slotNum] == MouseItemSlot.Instance.itemSlot)
            {
                int canAddItemNum = 64 - itemSlot[slotNum].itemNum;
                if (MouseItemSlot.Instance.itemSlot.itemNum <= canAddItemNum)
                {
                    itemSlot[slotNum] += MouseItemSlot.Instance.itemSlot;
                    MouseItemSlot.Instance.ClearItemSlot();
                }
                else
                {
                    itemSlot[slotNum].itemNum = 64;
                    MouseItemSlot.Instance.itemSlot -= canAddItemNum;
                }
            }
        }
        else
        {
            int itemCode = itemSlot[slotNum].itemCode;
            int itemNum = itemSlot[slotNum].itemNum;
            itemSlot[slotNum].Clear();
            if (slotNum < 27)
                AddInventoryItem(itemCode, itemNum, true);
            else
                AddInventoryItem(itemCode, itemNum, false);
        }

        SetItemSlotImage(slotNum);
        //MouseItemSlot.Instance.SetItemSlotImage();
    }
    public void RightClickSlot(int slotNum)
    {
        if (true == MouseItemSlot.Instance.itemSlot.empty)
        {
            int temp = (1 == itemSlot[slotNum].itemNum % 2) ? 1 : 0;
            int itemNum = itemSlot[slotNum].itemNum /= 2;
            itemSlot[slotNum].itemNum = itemNum + temp;
            if (0 != itemNum)
                MouseItemSlot.Instance.itemSlot.itemCode = itemSlot[slotNum].itemCode;

            MouseItemSlot.Instance.itemSlot.itemNum = itemNum;
        }
        else
        {
            if (true == itemSlot[slotNum].empty)
            {
                itemSlot[slotNum].itemCode = MouseItemSlot.Instance.itemSlot.itemCode;
                ++itemSlot[slotNum];
                --MouseItemSlot.Instance.itemSlot;
            }

            else if (false == itemSlot[slotNum].isMax && MouseItemSlot.Instance.itemSlot == itemSlot[slotNum])
            {
                ++itemSlot[slotNum];
                --MouseItemSlot.Instance.itemSlot;
            }
        }
        SetItemSlotImage(slotNum);
        //MouseItemSlot.Instance.SetItemSlotImage();
    }
}
