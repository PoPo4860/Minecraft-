
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
    // 시야내에 있는 청크만 활성화.
    // 플레이어가 청크에서 청크사이로 이동할때마다 호출.
    //
    // 이전에 활성화 되었던 청크 좌표값(int,int)와 지금 활성화 시킬 좌표값 을 비교해서
    // 좌표값이 겹칠경우 아무 동작을 안하고,
    // 이전에 활성화 되었던 좌표값에서 지금 활성화 시킬 좌표값이 없다면 청크 생성.(좌표값이 있다면 활성화)
    // 이제 활성화 시킬 좌표값에서 이전에 활성화 시켰던 좌표값이 없다면 이전에 활성화 시켰던 좌표값은 비활성화.
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

            // 두 좌표배열의 중복되는 좌표값을 null로 바꿔주는 반복문
            for (int currentNum = 0; currentNum < 37; ++currentNum)
            {
                for (int newNum = 0; newNum < 37; ++newNum)
                {
                    // 좌표값이 null인경우(중복좌표라 지워진 경우) 다음좌표를 검색.
                    if (newViewChinkCoord[newNum, 0] == null)
                    {
                        continue;
                    }
                    else
                    {
                        // 겹치는 좌표값이 있다면 좌표를 지워준다.
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
                // 좌표값이 null인경우(중복좌표라 지워진 경우) 다음좌표를 검색.
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

            // 플레이어의 현재 좌표값 갱신
            playerCurrentChounkCoord = newPlayerChunkcoord;
        }
    }
}
