using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utile
{
    static System.Diagnostics.Stopwatch calculationTime = new System.Diagnostics.Stopwatch();

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
    static public Vector3 PosNormaliz(Vector3 pos)
    {
        bool xCheck = (pos.x < 0);
        bool zCheck = (pos.z < 0);
        pos.x %= VoxelData.ChunkWidth;
        pos.y %= VoxelData.ChunkHeight;
        pos.z %= VoxelData.ChunkWidth;
        if (xCheck) pos.x += (VoxelData.ChunkWidth);
        if (zCheck) pos.z += (VoxelData.ChunkWidth);
        return pos;
    }
    static public Vector3Int Vector3ToVector3Int(Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }
}
