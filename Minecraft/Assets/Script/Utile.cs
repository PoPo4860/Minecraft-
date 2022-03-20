using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utile
{
    static System.Diagnostics.Stopwatch calculationTime = new System.Diagnostics.Stopwatch();

    public struct VoxelPosAndChunkCoord
    {
        public Vector3 VexelPos;
        public ChunkCoord chunkCoord;
        public VoxelPosAndChunkCoord(Vector3 newVexelPos, ChunkCoord newChunkCoord)
        {
            VexelPos = newVexelPos;
            chunkCoord = newChunkCoord;
        }
    }

    static public void TimeWatchStart()
    {
        calculationTime.Start();
    }
    static public void TimeWatchStop()
    {
        calculationTime.Stop();
        Debug.Log(calculationTime.ElapsedMilliseconds.ToString() + "ms");
        calculationTime.Reset();
    }


    /// <summary> 위치 정규화. position값을 넣으면 위치에 해당하는 청크의 상대좌표와 복셀위치를 반환한다.</summary>
    static public VoxelPosAndChunkCoord PosNormalization(in Vector3 pos)
    {
        Vector3 VoxelPos = new Vector3(
        pos.x % VoxelData.ChunkWidth,
        pos.y % VoxelData.ChunkHeight,
        pos.z % VoxelData.ChunkWidth);
        if (pos.x < 0) VoxelPos.x += (VoxelData.ChunkWidth);
        if (pos.z < 0) VoxelPos.z += (VoxelData.ChunkWidth);

        ChunkCoord chunkCoord = new ChunkCoord((int)pos.x / VoxelData.ChunkWidth, (int)pos.z / VoxelData.ChunkWidth);
        if (pos.x < 0) chunkCoord.x -= 1;
        if (pos.z < 0) chunkCoord.z -= 1;
        return new VoxelPosAndChunkCoord(VoxelPos, chunkCoord);
    }


    static public Vector3Int Vector3ToVector3Int(in Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }
    
}
