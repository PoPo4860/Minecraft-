using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    public InventoryUI playerInventory;
    public ItemSlotUI playerInventoryUI;
    public Furnace_UI furncaeUI;

    [SerializeField] private RectTransform inventorySlotUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject CraftingTableUI;
    [SerializeField] private GameObject FurnaceUI;

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
    public Coroutine StartUICorutine(IEnumerator corutine)
    {
        return StartCoroutine(corutine);
    }

    public void StopUICorutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
    public void ClearUI()
    {
        UI.SetActive(false);
        CraftingTableUI.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        FurnaceUI.gameObject.SetActive(false);
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
    public void ActiveFurnaceUI()
    {
        UI.SetActive(true);
        FurnaceUI.gameObject.SetActive(true);
        inventorySlotUI.transform.localPosition = new Vector3(0, 0, 0);
    }
}
