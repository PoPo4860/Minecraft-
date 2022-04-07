using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelState
{
    public ushort id;
    [System.NonSerialized] public Chunk chunk;
    [System.NonSerialized] public Vector3Int pos;
    [System.NonSerialized] public ChunkCoord chunkCoord;
    public BlockType properties
    {
        get { return CodeData.GetBlockInfo(id); }
    }

    public VoxelState(Chunk _chunk, Vector3Int _pos, ChunkCoord _chunkCoord, ushort _id = 0)
    {
        id = _id;
        chunk = _chunk;
        chunkCoord = _chunkCoord;
        pos = _pos;
    }

    public Vector3Int globalPos
    {
        get
        {
            return new Vector3Int(
                pos.x + chunkCoord.x,
                pos.y,
                pos.z + chunkCoord.z);
        }
    }
}

