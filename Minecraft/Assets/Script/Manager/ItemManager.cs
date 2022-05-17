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
                    UIManager.Instance.playerInventory.AddInventoryItem(objectList[i].itemCode, objectList[i].itemNum);
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

    public void AddDropItem(int itemCode, int itemNum, Vector3 dropPos, Vector3 vec = new Vector3())
    {
        // 비활성화된 드롭아이템이 있다면 재활용
        for (int i = 0; i < objectList.Count; ++i)
        {
            if (false == objectList[i].gameObject.activeSelf) 
            {
                ReSetDropItem(objectList[i], itemCode, itemNum, vec);
                objectList[i].gameObject.transform.position = dropPos;
                return;
            }
        }

        // 비활성화된 드롭 아이템이 없다면 새로 생성
        DropItem dropItem = Instantiate(dorpItemObject, dropPos, Quaternion.identity);
        dropItem.transform.SetParent(gameObject.transform);
        ReSetDropItem(dropItem, itemCode, itemNum, vec);
        objectList.Add(dropItem);
    }

    private void ReSetDropItem(DropItem dropItem,int itemCode, int itemNum, Vector3 vec)
    {
        dropItem.itemCode = itemCode;
        dropItem.itemNum = itemNum;
        dropItem.gameObject.SetActive(true);
        dropItem.rigi.AddForce(vec);
    }
}
