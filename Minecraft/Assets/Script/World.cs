using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static World instance = null;

    public Material TextureAtlas;
    public Material TextureAtlasTrans;
    [SerializeField] private GameObject PlayerObject;
    [HideInInspector] public int worldSeed;

    private readonly int worldSizeInChunks = 1;
    private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private ChunkCoord playerCurrentChounkCoord = new ChunkCoord(0,0);
    private readonly List<ChunkCoord> chunkCreateList = new List<ChunkCoord>();
    private Chunk currentCreateChunk;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static World Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        worldSeed = UnityEngine.Random.Range(0, 10000);
        GenerateWorld();
        UpdateChunksInViewRange(playerCurrentChounkCoord);
        chunkCreateList.Sort(new ChunkSort(playerCurrentChounkCoord));
    }
    private void Update()
    {
        ChunkCoord newPlayerChunkcoord = GetChunkCoordFromWorldPos(PlayerObject.transform.position);
        if (playerCurrentChounkCoord != newPlayerChunkcoord)
        {
            UpdateChunksInViewRange(newPlayerChunkcoord);
            chunkCreateList.Sort(new ChunkSort(playerCurrentChounkCoord));
        }
        CreateChunk();
    }

    private void CreateChunk()
    {
        if (currentCreateChunk != null)
        {
            if (currentCreateChunk.chunkState == Chunk.ChunkState.CoroutineStart)
                StartCoroutine(currentCreateChunk.CreateChunkMesh());
            else if (currentCreateChunk.chunkState == Chunk.ChunkState.CoroutineEnd)
                currentCreateChunk = null;
            else
                return;
        }
        if (chunkCreateList.Count != 0 && currentCreateChunk == null)
        {
            int index = chunkCreateList.Count - 1;
            currentCreateChunk = GetChunkFromCoord(chunkCreateList[index]);
            if (currentCreateChunk == null)
                currentCreateChunk = CreateNewChunk(chunkCreateList[index]);
            StartCoroutine(currentCreateChunk.CreateChunkMesh());
            chunkCreateList.RemoveAt(index);
        }
    }
    public void ChunkQueuePush(ChunkCoord coord)
    {
        if (false == chunkCreateList.Contains(coord))
        {
            chunkCreateList.Add(coord);
        }   
    }
    private void GenerateWorld()
    {
        for (int x = -worldSizeInChunks; x < worldSizeInChunks; x++)
        {
            for (int z = -worldSizeInChunks; z < worldSizeInChunks; z++)
            {
                CreateNewChunk(new ChunkCoord(x, z));
            }
        }
    }
    public Chunk CreateNewChunk(in Vector2Int chunkPos)
    {
        Chunk chunk = new Chunk(new ChunkCoord(chunkPos.x, chunkPos.y));
        chunks.Add(chunkPos, chunk);
        return chunk;
    }
    public Chunk CreateNewChunk(in ChunkCoord chunkPos)
    {
        Chunk chunk = new Chunk(chunkPos);
        chunks.Add(new Vector2Int(chunkPos.x, chunkPos.z), chunk);
        return chunk;
    }
    public Chunk GetChunkFromCoord(Vector2Int chunkPos)
    {
        chunks.TryGetValue(chunkPos, out Chunk getChunk);
        return getChunk;
    }
    public Chunk GetChunkFromCoord(ChunkCoord chunkPos)
    {
        chunks.TryGetValue(new Vector2Int(chunkPos.x, chunkPos.z), out Chunk getChunk);
        return getChunk;
    }
    public Chunk GetChunkFromPos (Vector3 pos)
    {
        Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(pos);
        
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
    private void UpdateChunksInViewRange(ChunkCoord newCoord)
    {
        int view = 10;
        for (int x = -view; x < view; ++x)
        {
            for (int z = -view; z < view; ++z)
            {
                ChunkCoord coord = new ChunkCoord(playerCurrentChounkCoord.x + x, playerCurrentChounkCoord.z + z);
                Chunk chunk = GetChunkFromCoord(coord);

                if (null == chunk)
                    ChunkQueuePush(coord);
                else if (Chunk.ChunkState.CoroutineStart == chunk.chunkState)
                    ChunkQueuePush(coord);
                else if (Chunk.ChunkState.CoroutineEnd == chunk.chunkState && chunk.ChunkObject.gameObject.activeSelf == false)
                    chunk.ChunkObject.gameObject.SetActive(true);

            }
        }
        // 플레이어의 현재 좌표값 갱신
        playerCurrentChounkCoord = newCoord;
    }
    public bool CheckBlockSolid(in Vector3 pos)
    {
        Utile.ChunkCoordInPos result =  Utile.GetCoordInVoxelPosFromWorldPos(pos);
        ushort blockCode = GetChunkFromCoord(result.chunkCoord).
            chunkMapData.GetVoxelState(result.voxelPos).id;
        return CodeData.GetBlockInfo(blockCode).isSolid;
    }
}

class ChunkSort : IComparer<ChunkCoord>
{
    public ChunkCoord coord;
    public ChunkSort(ChunkCoord _coord) //생성자
    {
        coord = _coord;
    }

    public int Compare(ChunkCoord a, ChunkCoord b)
    {
        float aDis = Vector2.Distance(new Vector2(coord.x, coord.z), new Vector2(a.x, a.z));
        float bDis = Vector2.Distance(new Vector2(coord.x, coord.z), new Vector2(b.x, b.z));
        if (aDis > bDis)
            return -1;
        else if (aDis < bDis)
            return 1;
        else
            return 0;
    }
}
