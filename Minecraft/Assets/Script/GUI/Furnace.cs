using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : ItemSlotUI
{
    private readonly int rawMaterialSlotNum = 0;
    private readonly int firewoodSlotNum = 1;
    private readonly int resultSlotNum = 2;

    private float combustionTime = 0f;
    private float combustionPower = 0f;
    private float combustionMaxPower = 0f;
    private bool checkCoroutineIsRun = false;

    public RawImage fireImage;
    public RectTransform fireImageTrans;
    public RawImage dirImage;
    public RectTransform dirImageTrans;

    private float fireHeight = 0f;
    private float dirWidth = 0f;

    private Vector3Int pos;

    public void Start()
    {
        fireHeight = fireImageTrans.rect.height;
        dirWidth = dirImageTrans.rect.width;
    }

    public void OnDisable()
    {
        Rect fireRc = fireImage.uvRect;
        fireRc.y = 2;
        fireImage.uvRect = fireRc;

        Rect dirRc = dirImage.uvRect;
        dirRc.x = 2;
        dirImage.uvRect = dirRc;
    }
    public override void LeftClickSlot(int slotNum)
    {
        if (resultSlotNum == slotNum)
        {
            ResultSlotNumWork();
            SetCombustion();
        }
        
        if (rawMaterialSlotNum == slotNum)
        {
            LeftClickSlotWork(slotNum);
            SetCombustion();
        }

        if (firewoodSlotNum == slotNum)
        {
            LeftClickSlotWork(slotNum);
            SetCombustion();
        }
    }
    public override void RightClickSlot(int slotNum)
    {
        if (resultSlotNum != slotNum)
        {
            RightClickSlotWork(slotNum);
            SetCombustion();
        }
    }
    public IEnumerator CombustionWrok()
    {
        while (0f < combustionPower ||
               0f != combustionTime) 
        {
            yield return new WaitForSeconds(0.5f);
            checkCoroutineIsRun = true;

            if (0f < combustionPower)
                combustionPower -= 0.5f;
            SetCombustionPower();
            

            if (0f < combustionPower && true == CanCombustion())
                combustionTime += 0.5f;

            else if (0f < combustionTime)
                combustionTime -= 0.5f;

            if (combustionTime == FurnaceDafa.combustionMaxTime)
            {
                combustionTime = 0f;
                int resultCode = FurnaceDafa.GetBakeResultCode(itemSlot[rawMaterialSlotNum].itemCode);
                if (0 != resultCode)
                {
                    --itemSlot[rawMaterialSlotNum];
                    itemSlot[resultSlotNum].itemCode = resultCode;
                    ++itemSlot[resultSlotNum];
                }
            }

            SetImage();
        }
        checkCoroutineIsRun = false;
    }
    private void SetImage()
    {
        float fireUvValue = -1 + (combustionPower / combustionMaxPower);
        Rect fireRc = fireImage.uvRect;
        fireRc.y = fireUvValue;
        fireImage.uvRect = fireRc;

        float fireH = fireHeight * 2.5f - (fireHeight * -fireUvValue);
        Vector3 fireTrans = fireImageTrans.localPosition;
        fireTrans.y = fireH;
        fireImageTrans.localPosition = fireTrans;

        float dirUvValue = -1 + (combustionTime / FurnaceDafa.combustionMaxTime);
        Rect dirRc = dirImage.uvRect;
        dirRc.x = dirUvValue;
        dirImage.uvRect = dirRc;

        float dirH = 3f- (dirWidth * -dirUvValue);
        Vector3 dirTrans = dirImageTrans.localPosition;
        dirTrans.x = dirH;
        dirImageTrans.localPosition = dirTrans;
    }
    public void SetCombustion()
    {
        SetCombustionPower();
        if (checkCoroutineIsRun == false)
            GameManager.Instance.uiManager.StartUICorutine(CombustionWrok());
    }
    public void SetCombustionPower()
    {
        if (0f != combustionPower)
            return;

        float combustionCode = FurnaceDafa.GetCombustionValue(itemSlot[firewoodSlotNum].itemCode);
        int resultCode = FurnaceDafa.GetBakeResultCode(itemSlot[rawMaterialSlotNum].itemCode);
        if (0f != combustionCode && 0 != resultCode)
        {
            combustionPower = combustionCode;
            combustionMaxPower = combustionCode;
            --itemSlot[firewoodSlotNum];
        }
    }
    private bool CanCombustion()
    {
        int resultCode = FurnaceDafa.GetBakeResultCode(itemSlot[rawMaterialSlotNum].itemCode);
        if (0 != resultCode)
        {
            return (resultCode == itemSlot[resultSlotNum].itemCode) || 
                    0 == itemSlot[resultSlotNum].itemCode;
        }
        return false;
    }
    private void ResultSlotNumWork()
    {
        ItemSlot mouseSlot = GameManager.Instance.mouseItemSlot.itemSlot;
        if (false == itemSlot[resultSlotNum].empty)
        {// 
            if (true == mouseSlot.empty)
            {
                GameManager.Instance.mouseItemSlot.SwapItemSlot(ref itemSlot[resultSlotNum]);
            }
            else
            {
                int slotItemNum = itemSlot[resultSlotNum].itemNum;
                int mouseItemNum = 64 - mouseSlot.itemNum;
                if (mouseItemNum >= slotItemNum && mouseSlot == itemSlot[resultSlotNum])
                    mouseSlot += slotItemNum;
            }
        }
    }
}