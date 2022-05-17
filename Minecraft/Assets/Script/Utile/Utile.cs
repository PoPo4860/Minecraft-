using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utile
{
    static System.Diagnostics.Stopwatch calculationTime = new System.Diagnostics.Stopwatch();

    public struct ChunkCoordInPos
    {
        public Vector3 voxelPos;
        public ChunkCoord chunkCoord;
        public ChunkCoordInPos(Vector3 newVexelPos, ChunkCoord newChunkCoord)
         {
            voxelPos = newVexelPos;
            chunkCoord = newChunkCoord;
        }
    }

    static public void StartTimeWatch()
    {
        calculationTime.Start();
    }
    static public void StopTimeWatch()
    {
        calculationTime.Stop();
        Debug.Log(calculationTime.ElapsedMilliseconds.ToString() + "ms");
        calculationTime.Reset();
    }

    static public void IntSwap(ref int a, ref int b)
    {
        int tempInt = a;
        a = b;
        b = tempInt;
    }
    /// <summary> 위치 정규화. position값을 넣으면 위치에 해당하는 청크의 상대좌표와 복셀위치를 반환한다.</summary>
    static public ChunkCoordInPos GetCoordInVoxelPosFromWorldPos(in Vector3 pos)
    {
        Vector3 VoxelPos = new Vector3(
        pos.x % VoxelData.ChunkWidth,
        pos.y % VoxelData.ChunkHeight,
        pos.z % VoxelData.ChunkWidth);
        if (pos.x < 0 && VoxelPos.x != 0) VoxelPos.x += (VoxelData.ChunkWidth);
        if (pos.z < 0 && VoxelPos.z != 0) VoxelPos.z += (VoxelData.ChunkWidth);

        ChunkCoord chunkCoord = new ChunkCoord((int)pos.x / VoxelData.ChunkWidth, (int)pos.z / VoxelData.ChunkWidth);
        if (pos.x < 0 && VoxelPos.x != 0) chunkCoord.x -= 1;
        if (pos.z < 0 && VoxelPos.z != 0) chunkCoord.z -= 1;
        return new ChunkCoordInPos(VoxelPos, chunkCoord);
    }
    static public Vector3Int GetWorldPosFormCoordInVoxelPos(in ChunkCoord coord, in Vector3 pos)
    {
        int x = (coord.x * 16) + (int)pos.x;
        int y = (int)pos.y;
        int z = (coord.z * 16) + (int)pos.z;
        //if (coord.x < 0 && x != 0) x += -(VoxelData.ChunkWidth - 1);
        //if (coord.z < 0 && z != 0) z += -(VoxelData.ChunkWidth - 1);
        return new Vector3Int(x, y, z);
    }
    static public Vector3Int Vector3ToVector3Int(in Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }
    static public Voxel GetVoxelStateFromWorldPos(in Vector3 pos)
    {
        ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(pos);
        Vector3Int voxelPos = Utile.Vector3ToVector3Int(result.voxelPos);

        return World.Instance.GetChunkFromCoord(result.chunkCoord).chunkMapData.GetVoxelState(result.voxelPos);
    }
    
    static public void ModifyChunkDataFromWorldPos(Vector3 worldPos, int itemCode)
    {
        Voxel voxel = GetVoxelStateFromWorldPos(worldPos);
        ChunkCoordInPos result = GetCoordInVoxelPosFromWorldPos(worldPos);
        Vector3Int pos = Vector3ToVector3Int(result.voxelPos);
        World.Instance.GetChunkFromCoord(result.chunkCoord).
            ModifyChunkData(pos, (ushort)itemCode, voxel.directionVector);
    }
}
