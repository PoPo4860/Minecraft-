using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    List<DropItem> objectList = new List<DropItem>();
    public Transform playerTransform;
    public DropItem dorpItemObject;

    private static ItemManager instance;
    public static ItemManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (true == objectList[i].gameObject.activeSelf)
            {
                float dis = Vector3.Distance(objectList[i].transform.position, playerTransform.position);
                if (dis < 0.5f)
                {
                    PlayerInventory.Instance.AddInventoryItem(objectList[i].itemCode, objectList[i].itemNum);
                    objectList[i].gameObject.SetActive(false);
                }
            }
        }
    }
    public void AddDropItem(int itemCode, int itemNum, Vector3 pos)
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (false == objectList[i].gameObject.activeSelf) 
            {
                objectList[i].itemCode = itemCode;
                objectList[i].itemNum = itemNum;
                objectList[i].gameObject.transform.position = pos;
                objectList[i].gameObject.SetActive(true);
                return;
            }
        }
        DropItem obj = Instantiate(dorpItemObject, pos, Quaternion.identity);
        obj.transform.SetParent(gameObject.transform);
        obj.itemCode = itemCode;
        obj.itemNum = itemNum;
        obj.gameObject.SetActive(true);
        objectList.Add(obj);
    }
}
