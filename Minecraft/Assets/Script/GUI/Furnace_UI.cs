using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace_UI : MonoBehaviour
{
    [SerializeField] private Furnace furnaceObject;
    private readonly Dictionary<Vector3Int, Furnace> furnaceData = new Dictionary<Vector3Int, Furnace>();
    private Furnace currentFurnace = null;

    private void OnEnable()
    {
        Vector3 pos = GameManager.Instance.player.GetHighLightBlockPos();
        Vector3Int posInt = Utile.Vector3ToVector3Int(pos);
        furnaceData.TryGetValue(posInt, out Furnace furnace);
        currentFurnace = furnace;
        if (null == furnace)
        {
            Furnace newFurnace = Instantiate(furnaceObject, Vector2.zero, Quaternion.identity);
            newFurnace.SetWorldPos(posInt);
            newFurnace.transform.SetParent(transform, false);
            newFurnace.name = $"Furnace [{posInt.x}, {posInt.y}, {posInt.z}]";
            furnaceData.Add(posInt, newFurnace);
            currentFurnace = newFurnace;
            newFurnace.gameObject.SetActive(true);
        }
        else
        {
            furnace.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        currentFurnace.gameObject.SetActive(false);
    }
    public void DeletFurnace(in Vector3Int pos)
    {
        furnaceData.TryGetValue(pos, out Furnace furnace);
        furnaceData.Remove(pos);
        furnace?.Destory(pos);
        Destroy(furnace?.gameObject);
    }
}
