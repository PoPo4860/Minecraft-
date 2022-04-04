using UnityEngine;

[System.Serializable]
public class VoxelState
{
    public ushort id;
    [System.NonSerialized] private byte _light;
    [System.NonSerialized] public Chunk chunk;
    [System.NonSerialized] public VoxelNeighbours neighbours;
    [System.NonSerialized] public Vector3Int pos;
    public byte light
    {
        get { return _light; }
        set { 
            if(value != _light)
            {
                if (_light > 1)
                    PropogateLight();
            }
        }
    }
    public byte castLight
    {
        get
        {
            int lightLevel = _light - properties.opacity - 1;
            if (lightLevel < 0) lightLevel = 0;
            return (byte)lightLevel;
        }
    }

    public void PropogateLight()
    {
        if (light < 2)
            return;

        for (int p = 0; p < 6; ++p)
        {
            if (null != neighbours[p])
            {
                if (neighbours[p].light < castLight)
                    neighbours[p].light = castLight;
            }
        }
    }

    public BlockType properties
    {
        get { return CodeData.GetBlockInfo(id); }
    }
    public float lightAsFloat
    {
        get { return light * VoxelData.unitOfLight; }
    }
    public VoxelState(Chunk _chunk, Vector3Int _pos, ushort _id = 0)
    {
        neighbours = new VoxelNeighbours(this);
        id = _id;
        chunk = _chunk;
        pos = _pos;
        _light = 0;
    }

    public Vector3Int globalPos
    {
        get
        {
            return new Vector3Int(
                pos.x + (int)chunk.ChunkObject.transform.position.x,
                pos.y,
                pos.x + (int)chunk.ChunkObject.transform.position.x);
        }
    }
}

public class VoxelNeighbours
{
    public readonly VoxelState parent;
    public VoxelNeighbours(VoxelState _parent) { parent = _parent; }

    private VoxelState[] _neighbours = new VoxelState[6];
    public int Length { get { return _neighbours.Length; } }
    
    public VoxelState this[int index]
    {

        get
        {
            if (_neighbours[index] == null)
            {
                _neighbours[index] = World.Instance.GetVoxelFromWorldPos(parent.globalPos+VoxelData.faceChecks[index]);
            }
            return _neighbours[index]; 
        }
        set { _neighbours[index] = value; }
    }
    void ReturnNeighbour(int index)
    {
        if (_neighbours[index] == null)
            return;

        if (_neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] != parent)
            _neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] = parent;
    }
}
