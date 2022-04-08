using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelState
{
    public ushort id;
    private readonly Vector3Int voxelPos;
    private readonly ChunkMapData chunkMapData;
    public BlockType properties
    {
        get { return CodeData.GetBlockInfo(id); }
    }

    public Vector3Int gobalPos
    {
        get {
            return new Vector3Int(
        (chunkMapData.coord.x * VoxelData.ChunkWidth) + voxelPos.x,
        voxelPos.y,
        (chunkMapData.coord.z * VoxelData.ChunkWidth) + voxelPos.z);
        }
    }

    public VoxelState(ChunkMapData _chunkMapData, Vector3Int _voxelPos, ushort _id = 0)
    {
        voxelPos = _voxelPos;
        chunkMapData = _chunkMapData;
        id = _id;
    }
}

