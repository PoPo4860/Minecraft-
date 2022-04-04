using UnityEngine;

public static class Lighting
{
    public static void RecalculateNaturaLight(Chunk chunk)
    {
        for(int x = 0; x < VoxelData.ChunkWidth; ++x)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; ++z)
            {
                CastNaturaLight(chunk, x, z, VoxelData.ChunkHeight - 1);
            }
        }
    }

    public static void CastNaturaLight(Chunk chunk, int x, int z, int startY)
    {
        if (startY > VoxelData.ChunkHeight - 1)
        {
            startY = VoxelData.ChunkHeight - 1;
            Debug.Log("���󿡼� ������ ĳ�����̶�� ǥ���϶�� �ߴµ� �� �������� �������� �ϴ� ǥ���غ��� �α�");
        }

        bool obstructed = false;

        for (int y = startY; y > -1; --y)
        {
            VoxelState voxel = chunk.voxelMap[x, y, z];
            
            if (obstructed)
                voxel.light = 0;
            else if (voxel.properties.opacity > 0)
            {
                voxel.light = 0;
                obstructed = true;
            }
            else
            {
                voxel.light = 15;
            }
        }
    }
}
