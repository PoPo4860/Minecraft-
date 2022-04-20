using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    List<DropItem> objectList = new List<DropItem>();
    public Transform playerTransform;
    public DropItem dorpItemObject;

    void Update()
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (true == objectList[i].canIsItemGet)
            {
                float dis = Vector3.Distance(objectList[i].transform.position, playerTransform.position);
                if (dis < 0.3f)
                {
                    GameManager.Instance.playerInventory.AddInventoryItem(objectList[i].itemCode, objectList[i].itemNum);
                    objectList[i].gameObject.SetActive(false);
                }
            }
        }
    }
    public void FixedUpdate()
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (true == objectList[i].canIsItemGet)
            {
                float dis = Vector3.Distance(objectList[i].transform.position, playerTransform.position);
                float grabDis = 2.0f;
                if (dis < grabDis)
                {
                    Vector3 vector = playerTransform.position - objectList[i].transform.position;
                    vector.y = 0;
                    vector.Normalize();
                    float speed = ((-dis + grabDis + 0.1f) * 3);
                    vector *= Time.fixedDeltaTime * speed;
                    objectList[i].rigi.SetVelocity(vector);
                }
                else
                {
                    objectList[i].rigi.SetVelocity(Vector3.zero);
                }
            }
        }
    }

    public void AddDropItem(int itemCode, int itemNum, Vector3 pos, Vector3 vec = new Vector3())
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (false == objectList[i].gameObject.activeSelf) 
            {
                objectList[i].itemCode = itemCode;
                objectList[i].itemNum = itemNum;
                objectList[i].gameObject.transform.position = pos;
                objectList[i].rigi.SetVelocity(vec);
                objectList[i].gameObject.SetActive(true);
                return;
            }
        }
        DropItem obj = Instantiate(dorpItemObject, pos, Quaternion.identity);
        obj.transform.SetParent(gameObject.transform);
        obj.itemCode = itemCode;
        obj.itemNum = itemNum;
        obj.rigi.SetVelocity(vec);
        obj.gameObject.SetActive(true);
        objectList.Add(obj);
    }
}
