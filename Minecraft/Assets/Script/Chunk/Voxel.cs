using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Voxel
{
    private ushort _id;

    public ushort id
    {
        get { return _id; }
        set 
        { 
            _id = value;
            if (0 == value)
                _direction = Vector2Int.zero;
        }
    }
    private Vector2Int _direction = new Vector2Int();

    public int direction
    {
        get {
            if (-1 == _direction.y)
                return 0;
            if (+1 == _direction.y)
                return 1;
            if (-1 == _direction.x)
                return 2;
            if (+1 == _direction.x)
                return 3;
            return 0;
            }
    }
    public Vector2Int directionVector
    {
        set
        {
            _direction = value;
        }
        get
        {
            return _direction;
        }
    }
    public BlockType blockProperties
    {
        get { return CodeData.GetBlockInfo(id); }
    }
    public ItemType itemProperties
    {
        get { return CodeData.GetItemInfo(id); }
    }

    public Voxel(ushort _id = 0)
    {
        this._id = _id;
    }
}

