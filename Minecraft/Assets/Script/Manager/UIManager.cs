using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject UI;
    public InventoryUI playerInventory;
    public ItemSlotUI playerInventoryUI;

    public RectTransform inventorySlotUI;
    public GameObject inventoryUI;
    public GameObject CraftingTableUI;
    public static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearUI()
    {
        UI.SetActive(false);
        CraftingTableUI.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
    }
    public void ActiveIventoryUI()
    {
        UI.SetActive(true);
        inventoryUI.gameObject.SetActive(true);
        inventorySlotUI.transform.localPosition = new Vector3(0, 33, 0);
    }

    public void ActiveCraftingUI()
    {
        UI.SetActive(true);
        CraftingTableUI.gameObject.SetActive(true);
        inventorySlotUI.transform.localPosition = new Vector3(0, 0, 0);
    }
}
