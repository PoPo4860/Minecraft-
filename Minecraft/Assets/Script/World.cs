
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    private int worldSizeInChunks = 5;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < worldSizeInChunks; x++)
        {
            for (int z = 0; z < worldSizeInChunks; z++)
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

        //foreach(Vector2Int vec in vector)
        //{
        //    GetChunk(vec)?.CreateMeshData();
        //}
    }

    public Chunk GetChunk(Vector2Int chunkPos)
    {
        Chunk getChunk;
        chunks.TryGetValue(chunkPos, out getChunk);
        return getChunk;
    }
}
