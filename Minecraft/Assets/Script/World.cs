using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material TextureAtlas;
    public Material TextureAtlasTrans;
    [SerializeField] private GameObject PlayerObject;
    [HideInInspector] public int worldSeed;
    private int worldSizeInChunks = 5;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private ChunkCoord playerCurrentChounkCoord = new ChunkCoord(0,0);
    private Queue<Chunk> chunkUpdataList = new Queue<Chunk>();
    private void Start()
    {
        worldSeed = Random.Range(0, 10000);
        GenerateWorld();
    }
    private void Update()
    {
        UpdateChunksInViewRange();
        if (chunkUpdataList.Count != 0)
        {
            Chunk chunk = chunkUpdataList.Peek();
            if(chunk.chunkState == Chunk.ChunkState.CoroutineStart)
            {
                StartCoroutine(chunk.CreateChunkMesh());
            }
            else if (chunk.chunkState == Chunk.ChunkState.CoroutineEnd)
            {
                chunkUpdataList.Dequeue();
            }
        }
    }
    public void ChunkQueuePush(Chunk chunk)
    {
        if (false == chunkUpdataList.Contains(chunk))
        {
            chunkUpdataList.Enqueue(chunk);
        }   
    }
    private void GenerateWorld()
    {
        for (int x = -worldSizeInChunks; x < worldSizeInChunks; x++)
        {
            for (int z = -worldSizeInChunks; z < worldSizeInChunks; z++)
            {
                ChunkQueuePush(CreateNewChunk(x, z));
            }
        }
    }
    public Chunk CreateNewChunk(in int x, in int z)
    {
        Chunk chunk = new Chunk(new ChunkCoord(x, z), this);
        chunks.Add(new Vector2Int(x, z), chunk);
        return chunk;
    }
    public Chunk CreateNewChunk(in Vector2Int chunkPos)
    {
        Chunk chunk = new Chunk(new ChunkCoord(chunkPos.x, chunkPos.y), this);
        chunks.Add(chunkPos, chunk);
        return chunk;
    }
    public Chunk GetChunkFromCoord(Vector2Int chunkPos)
    {
        chunks.TryGetValue(chunkPos, out Chunk getChunk);
        return getChunk;
    }
    public Chunk GetChunkFromPos (Vector3 pos)
    {
        Utile.VoxelPosAndChunkCoord result = Utile.PosNormalization(pos);
        chunks.TryGetValue(new Vector2Int(result.chunkCoord.x, result.chunkCoord.z), out Chunk getChunk);
        return getChunk;
    }
    private ChunkCoord GetChunkCoordFromWorldPos(in Vector3 worldPos)
    {
        int x = (int)(worldPos.x / VoxelData.ChunkWidth);
        int z = (int)(worldPos.z / VoxelData.ChunkWidth);

        if (worldPos.x < 0) --x;
        if (worldPos.z < 0) --z;
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
                    GetChunkFromCoord(new Vector2Int(x, z)).ChunkObject.SetActive(false);
                }
                if (newViewChinkCoord[i, 0] != null)
                {
                    int x = (int)newViewChinkCoord[i, 0];
                    int z = (int)newViewChinkCoord[i, 1];
                    Chunk chunk = GetChunkFromCoord(new Vector2Int(x, z));
                    if (chunk == null)
                    {
                        Chunk newChunk = new Chunk(new ChunkCoord(x, z), this);
                        ChunkQueuePush(newChunk);
                        chunks.Add(new Vector2Int(x, z), newChunk);
                    }
                    else
                    {
                        if(chunk.chunkState == Chunk.ChunkState.CoroutineStart)
                        {
                            ChunkQueuePush(chunk);
                        }
                        else
                        {
                            chunk.ChunkObject.SetActive(true);
                        }
                    }
                }
            }

            // 플레이어의 현재 좌표값 갱신
            playerCurrentChounkCoord = newPlayerChunkcoord;
        }
    }
    public bool CheckBlockSolid(in Vector3 pos)
    {
        Utile.VoxelPosAndChunkCoord result =  Utile.PosNormalization(pos);

        return 0 != GetChunkFromCoord(new Vector2Int(result.chunkCoord.x, result.chunkCoord.z)).GetBlockID(result.VexelPos);
    }
}
