using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public InventoryUI playerInventory;
    public ItemSlotUI playerInventoryUI;
    public InventoryCraftingUI inventoryCraftingUI;

    public static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
