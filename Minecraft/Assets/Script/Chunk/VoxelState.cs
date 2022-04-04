[System.Serializable]
public class VoxelState
{
    public ushort id;
    [System.NonSerialized] private byte _light;
    [System.NonSerialized] public Chunk chunk;
    [System.NonSerialized] public VoxelState[] neighbours = new VoxelState[6];
    public byte light
    {
        get { return _light; }
        set { 
            if(value != _light)
            {
                if (_light > 1)
                    chunk.AddForLightForPropogation(this);
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
    public VoxelState(Chunk _chunk, ushort _id = 0)
    {
        id = _id;
        chunk = _chunk;
        _light = 0;
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
        get { return _neighbours[index]; }
        set { _neighbours[index] = value; }
    }

}
