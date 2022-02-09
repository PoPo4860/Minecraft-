
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material Atlas;
    public GameObject PlayerObject;
    private int worldSizeInChunks = 10;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private ChunkCoord playerCurrentChounkCoord = new ChunkCoord(0,0);

    //
    // �þ߳��� �ִ� ûũ�� Ȱ��ȭ.
    // �÷��̾ ûũ���� ûũ���̷� �̵��Ҷ����� ȣ��.
    //
    // ������ Ȱ��ȭ �Ǿ��� ûũ ��ǥ��(int,int)�� ���� Ȱ��ȭ ��ų ��ǥ�� �� ���ؼ�
    // ��ǥ���� ��ĥ��� �ƹ� ������ ���ϰ�,
    // ������ Ȱ��ȭ �Ǿ��� ��ǥ������ ���� Ȱ��ȭ ��ų ��ǥ���� ���ٸ� ûũ ����.(��ǥ���� �ִٸ� Ȱ��ȭ)
    // ���� Ȱ��ȭ ��ų ��ǥ������ ������ Ȱ��ȭ ���״� ��ǥ���� ���ٸ� ������ Ȱ��ȭ ���״� ��ǥ���� ��Ȱ��ȭ.
    //

    private void Start()
    {
        GenerateWorld();
    }

    private void Update()
    {
        UpdateChunksInViewRange();
    }
    private void GenerateWorld()
    {
        for (int x = -worldSizeInChunks; x < worldSizeInChunks; x++)
        {
            for (int z = -worldSizeInChunks; z < worldSizeInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    private void CreateNewChunk(int x, int z)
    {
        chunks.Add(new Vector2Int(x, z), new Chunk(new ChunkCoord(x, z), this));
        //Vector2Int[] vector = new Vector2Int[4]
        //{
        //    new Vector2Int(x - 1, z),
        //    new Vector2Int(x + 1, z),
        //    new Vector2Int(x, z - 1),
        //    new Vector2Int(x, z + 1)
        //};
        //
        //foreach(Vector2Int vec in vector)
        //{
        //    GetChunk(vec)?.CreateMeshData();
        //}
    }

    public Chunk GetChunk(Vector2Int chunkPos)
    {
        chunks.TryGetValue(chunkPos, out Chunk getChunk);
        return getChunk;
    }

    private ChunkCoord GetChunkCoordFromWorldPos(in Vector3 worldPos)
    {
        int x = (int)(worldPos.x / VoxelData.ChunkWidth);
        int z = (int)(worldPos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }
    private void UpdateChunksInViewRange()
    {
        ChunkCoord newPlayerChunkcoord = GetChunkCoordFromWorldPos(PlayerObject.transform.position);
        if (playerCurrentChounkCoord != newPlayerChunkcoord)
        {
            int?[,] currentViewChinkCoord = new int?[37, 2]{
                              {-1,-3 },{+0,-3 },{+1,-3 },
                     {-2,-2 },{-1,-2 },{+0,-2 },{+1,-2 },{+2,-2 },
            {-3,-1 },{-2,-1 },{-1,-1 },{+0,-1 },{+1,-1 },{+2,-1 },{+3,-1 },
            {-3,+0 },{-2,+0 },{-1,+0 },{+0,+0 },{+1,+0 },{+2,+0 },{+3,+0 },
            {-3,+1 },{-2,+1 },{-1,+1 },{+0,+1 },{+1,+1 },{+2,+1 },{+3,+1 },
                     {-2,+2 },{-1,+2 },{+0,+2 },{+1,+2 },{+2,+2 },
                              {-1,+3 },{+0,+3 },{+1,+3 }};
            int?[,] newViewChinkCoord = new int?[37, 2]{
                              {-1,-3 },{+0,-3 },{+1,-3 },
                     {-2,-2 },{-1,-2 },{+0,-2 },{+1,-2 },{+2,-2 },
            {-3,-1 },{-2,-1 },{-1,-1 },{+0,-1 },{+1,-1 },{+2,-1 },{+3,-1 },
            {-3,+0 },{-2,+0 },{-1,+0 },{+0,+0 },{+1,+0 },{+2,+0 },{+3,+0 },
            {-3,+1 },{-2,+1 },{-1,+1 },{+0,+1 },{+1,+1 },{+2,+1 },{+3,+1 },
                     {-2,+2 },{-1,+2 },{+0,+2 },{+1,+2 },{+2,+2 },
                              {-1,+3 },{+0,+3 },{+1,+3 }};
            for (int i = 0; i < 37; ++i)
            {
                currentViewChinkCoord[i, 0] += playerCurrentChounkCoord.x;
                currentViewChinkCoord[i, 1] += playerCurrentChounkCoord.z;
                newViewChinkCoord[i, 0] += newPlayerChunkcoord.x;
                newViewChinkCoord[i, 1] += newPlayerChunkcoord.z;
            }

            // �� ��ǥ�迭�� �ߺ��Ǵ� ��ǥ���� null�� �ٲ��ִ� �ݺ���
            for (int currentNum = 0; currentNum < 37; ++currentNum)
            {
                for (int newNum = 0; newNum < 37; ++newNum)
                {
                    // ��ǥ���� null�ΰ��(�ߺ���ǥ�� ������ ���) ������ǥ�� �˻�.
                    if (newViewChinkCoord[newNum, 0] == null)
                    {
                        continue;
                    }
                    else
                    {
                        // ��ġ�� ��ǥ���� �ִٸ� ��ǥ�� �����ش�.
                        if (newViewChinkCoord[newNum, 0] == currentViewChinkCoord[currentNum, 0] &&
                            newViewChinkCoord[newNum, 1] == currentViewChinkCoord[currentNum, 1])
                        {
                            newViewChinkCoord[newNum, 0] = null;
                            newViewChinkCoord[newNum, 1] = null;
                            currentViewChinkCoord[currentNum, 0] = null;
                            currentViewChinkCoord[currentNum, 1] = null;
                            break;
                        }
                    }
                }
                // ��ǥ���� null�ΰ��(�ߺ���ǥ�� ������ ���) ������ǥ�� �˻�.
                if (currentViewChinkCoord[currentNum, 0] == null)
                {
                    continue;
                }
            }

            for (int i = 0; i < 37; ++i)
            {
                if (currentViewChinkCoord[i, 0] != null)
                {
                    int x = (int)currentViewChinkCoord[i, 0];
                    int z = (int)currentViewChinkCoord[i, 1];
                    GetChunk(new Vector2Int(x, z)).ChunkObject.SetActive(false);
                }
                if (newViewChinkCoord[i, 0] != null)
                {
                    int x = (int)newViewChinkCoord[i, 0];
                    int z = (int)newViewChinkCoord[i, 1];
                    Chunk chunk = GetChunk(new Vector2Int(x, z));
                    if (chunk == null)
                    {
                        chunks.Add(new Vector2Int(x, z), new Chunk(new ChunkCoord(x, z), this));
                    }
                    else
                    {
                        chunk.ChunkObject.SetActive(true);
                    }
                }
            }

            // �÷��̾��� ���� ��ǥ�� ����
            playerCurrentChounkCoord = newPlayerChunkcoord;
        }
    }
}
