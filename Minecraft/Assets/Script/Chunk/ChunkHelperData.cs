using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkHelperData
{
    public readonly static int[] vertexData = new int[] { 0, 1, 2, 2, 1, 3 };
    public readonly static int[,] vectorCoord = new int[4, 2] { { -1, 0 }, { +1, 0 }, { 0, -1 }, { 0, +1 } };
    public readonly static Vector3Int[] oreProximityPos = new Vector3Int[26]
    {
        new Vector3Int(1, 1, 1),new Vector3Int(1, 1, 0),new Vector3Int(1, 1,-1),
        new Vector3Int(1, 0, 1),new Vector3Int(1, 0, 0),new Vector3Int(1, 0,-1),
        new Vector3Int(1,-1, 1),new Vector3Int(1,-1, 0),new Vector3Int(1,-1,-1),

        new Vector3Int(-1, 1, 1),new Vector3Int(-1, 1, 0),new Vector3Int(-1, 1,-1),
        new Vector3Int(-1, 0, 1),new Vector3Int(-1, 0, 0),new Vector3Int(-1, 0,-1),
        new Vector3Int(-1,-1, 1),new Vector3Int(-1,-1, 0),new Vector3Int(-1,-1,-1),

        new Vector3Int(0, 1, 1),new Vector3Int(0,1,0),new Vector3Int(0, 1,-1),
        new Vector3Int(0, 0, 1),                      new Vector3Int(0, 0,-1),
        new Vector3Int(0,-1, 1),new Vector3Int(0,-1,0),new Vector3Int(0,-1,-1)
    };
    public readonly static int[,] treePos = new int[6, 25]
    {
        {   -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, CodeData.BLOCK_OakTree, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1 },

        {   -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, CodeData.BLOCK_OakTree, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1 },

        {   CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_OakTree, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf     },

        {   CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_OakTree, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf,
            CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf, CodeData.BLOCK_Leaf     },
        
        {   -1, -1, -1, -1, -1 ,
            -1, CodeData.BLOCK_Leaf ,CodeData.BLOCK_Leaf     ,CodeData.BLOCK_Leaf ,-1 ,
            -1, CodeData.BLOCK_Leaf ,CodeData.BLOCK_OakTree  ,CodeData.BLOCK_Leaf ,-1 ,
            -1, CodeData.BLOCK_Leaf ,CodeData.BLOCK_Leaf     ,CodeData.BLOCK_Leaf ,-1 ,
            -1, -1, -1, -1, -1 },
        
        {   -1, -1,                  -1,                   -1,                  -1,
            -1, -1,                  CodeData.BLOCK_Leaf , -1,                  -1,
            -1, CodeData.BLOCK_Leaf ,CodeData.BLOCK_Leaf ,CodeData.BLOCK_Leaf , -1,
            -1, -1,                  CodeData.BLOCK_Leaf , -1,                  -1,
            -1, -1,                  -1,                   -1,                  -1 },
    };
    public static Vector2Int VectorCoord(in int num, in ChunkCoord coord) => new Vector2Int(coord.x + vectorCoord[num, 0], coord.z + vectorCoord[num, 1]);
    public static Vector3Int VectorBlock(in Vector3 voxelFacePos, in int i) => i switch
    {
        0 => new Vector3Int(VoxelData.ChunkWidth - 1, (int)voxelFacePos.y, (int)voxelFacePos.z),
        1 => new Vector3Int(0, (int)voxelFacePos.y, (int)voxelFacePos.z),
        2 => new Vector3Int((int) voxelFacePos.x, (int) voxelFacePos.y, VoxelData.ChunkWidth - 1),
        _ => new Vector3Int((int)voxelFacePos.x, (int)voxelFacePos.y, 0),
    };

    public static bool Asd(in Vector3 voxelPos, in int faceNum) => faceNum switch
    {
        0 => (voxelPos.x <= -1),
        1 => (voxelPos.x >= VoxelData.ChunkWidth),
        2 => (voxelPos.z <= -1),
        3 => (voxelPos.z >= VoxelData.ChunkWidth),
        4 => (voxelPos.y <= -1 || voxelPos.y >= VoxelData.ChunkHeight),
        _ => false,
    };
}
