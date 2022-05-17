using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkMapData
{
    public readonly Voxel[,,] voxelMap =
    new Voxel[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public ChunkCoord coord;

    private readonly Queue<Vector3Int> createOrePos = new Queue<Vector3Int>();
    private readonly Queue<Vector3Int> createTreePos = new Queue<Vector3Int>();
    private World world
    {
        get { return World.Instance; }
    }

    public ChunkMapData(ChunkCoord _coord)
    {
        coord = _coord;
        for (int y = VoxelData.ChunkHeight-1; y >= 0; --y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    Vector3Int voxelPos = new Vector3Int(x, y, z);
                    ushort id = PopulateBlock(voxelPos);
                    voxelMap[x, y, z] = new Voxel(id);
                }
            }
        }
        SetOre();
        SetTree();
    }

    private ushort PopulateBlock(in Vector3Int pos)
    {
        if (60 < pos.y)
            return CodeData.BLOCK_Air;
        if (0 == pos.y)
            return CodeData.BLOCK_BedRock;

        float terrainResult = Perlin.GetPerlinNoiseTerrain(coord, world.worldSeed, pos);
        if (terrainResult < 0)
            return CodeData.BLOCK_Air;

        float cavePerlin = Perlin.GetPerlinNoiseCave(coord, world.worldSeed, pos);
        if (0.499f < cavePerlin && cavePerlin < 0.5f)
            createOrePos.Enqueue(pos);

        if (Perlin.GetPerlinNoiseCave(coord, world.worldSeed, pos) < 0.5f)
            return CodeData.BLOCK_Air;

        if (2 < terrainResult)
            return CodeData.BLOCK_Stone;

        if (0 == voxelMap[pos.x, pos.y + 1, pos.z].id) 
        {
            if (0 == Random.Range(0, 100))
                createTreePos.Enqueue(new Vector3Int(pos.x, pos.y + 1, pos.z));
            return CodeData.BLOCK_Grass;
        }

        return CodeData.BLOCK_Dirt;
    }
    private void SetOre(in Vector3Int basePos, in ushort blockCode, in int randomValue)
    {
        voxelMap[basePos.x, basePos.y, basePos.z].id = blockCode;
        foreach (Vector3Int pos in ChunkHelperData.oreProximityPos)
        {
            int posX = basePos.x + pos.x;
            int posY = basePos.y + pos.y;
            int posZ = basePos.z + pos.z;
            if (posX < 0 || posY < 2 || posZ < 0)
                continue;

            if (VoxelData.ChunkWidth - 1 < posX || VoxelData.ChunkHeight - 1 < posY || VoxelData.ChunkWidth - 1 < posZ)
                continue;

            if (voxelMap[posX, posY, posZ].id == CodeData.BLOCK_Stone && 0 == Random.Range(0, randomValue))
                voxelMap[posX, posY, posZ].id = blockCode;
        }
    }
    private void SetOre()
    {
        while (0 != createOrePos.Count)
        {
            Vector3Int basePos = createOrePos.Dequeue();

            if (20 <= basePos.y && 0 == Random.Range(0, 2))
                SetOre(basePos, CodeData.BLOCK_Coal, 2);

            else if (5 <= basePos.y && basePos.y < 30 && 0 == Random.Range(0, 4))
                SetOre(basePos, CodeData.BLOCK_Iron, 3);

            else if (1 < basePos.y && basePos.y < 10 && 0 == Random.Range(0, 5))
                SetOre(basePos, CodeData.BLOCK_Diamond, 7);
        }
    }
    private void SetTree()
    {
        while (0 != createTreePos.Count)
        {
            Vector3Int pos = createTreePos.Dequeue();

            if (pos.x < 2 || VoxelData.ChunkWidth - 3 < pos.x)
                continue;
            if (pos.z < 2 || VoxelData.ChunkWidth - 3 < pos.z)
                continue;

            bool isCanTree = true;
            for (int y = 0; y < 6; ++y)
            {
                for (int i = 0; i < 25; ++i)
                {
                    if (-1 == ChunkHelperData.treePos[y, i])
                        continue;

                    int posX = pos.x + ((i % 5) - 2);
                    int posY = pos.y + y;
                    int posZ = pos.z + ((i / 5) - 2);
                    int blockCode = GetVoxelState(new Vector3Int(posX, posY, posZ)).id;
                    if (CodeData.BLOCK_Air != blockCode && CodeData.BLOCK_Leaf != blockCode)
                    {
                        isCanTree = false;
                        break;
                    }
                }
                if (false == isCanTree)
                    break;
            }

            if (false == isCanTree)
                continue;

            for (int y = 0; y < 6; ++y)
            {
                for (int i = 0; i < 25; ++i)
                {
                    int posX = pos.x + ((i % 5) - 2);
                    int posY = pos.y + y;
                    int posZ = pos.z + ((i / 5) - 2);
                    if (-1 != ChunkHelperData.treePos[y, i])
                        voxelMap[posX, posY, posZ].id = (ushort)ChunkHelperData.treePos[y, i];
                }
            }
        }
    }

    public Voxel GetVoxelState(in Vector3 voxelPos)
    {
        for (int i = 0; i < 4; ++i)
        {
            if (ChunkHelperData.Asd(voxelPos, i))
            {
                Vector3Int worldPos =  Utile.GetWorldPosFormCoordInVoxelPos(coord, voxelPos);
                Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(worldPos);
                return world.GetChunkFromCoord(result.chunkCoord)?.chunkMapData.GetVoxelState(result.voxelPos);
            }
        }

        if (ChunkHelperData.Asd(voxelPos, 4))
            return null;
        return voxelMap[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z];
    }
    public void SetVoxelState(in Vector3Int pos, in ushort _id, in Vector2Int dir)
    {
        if (voxelMap[pos.x, pos.y, pos.z].id != _id)
        {
            voxelMap[pos.x, pos.y, pos.z].id = _id;
            voxelMap[pos.x, pos.y, pos.z].directionVector = dir;
        }
    }
}
