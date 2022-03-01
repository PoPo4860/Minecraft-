using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkCoord
{
    public int x;
    public int z;
    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public static bool operator ==(ChunkCoord a, ChunkCoord b)
    {
        if (a.x == b.x && a.z == b.z)
        {
            return true;
        }
        return false;
    }
    public static bool operator !=(ChunkCoord a, ChunkCoord b)
    {
        if (a.x != b.x || a.z != b.z)
        {
            return true;
        }
        return false;
    }
    public override bool Equals(object op1)
    {
        return (x == ((ChunkCoord)op1).x && z == ((ChunkCoord)op1).z);
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
public class Chunk
{
    private ushort[,,] voxelMap =
        new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public World world;
    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();

    public GameObject ChunkObject; // ûũ�� ������ ��� ���ӿ�����Ʈ
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public ChunkCoord coord;
    public Chunk(ChunkCoord coord, World world)
    {
        this.coord = coord;
        this.world = world;
        ChunkObject = new GameObject();
        meshRenderer = ChunkObject.AddComponent<MeshRenderer>();
        meshFilter = ChunkObject.AddComponent<MeshFilter>();

        meshRenderer.material = world.Atlas;
        ChunkObject.transform.SetParent(world.transform);
        ChunkObject.transform.position =
            new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        ChunkObject.name = $"Chunk [{coord.x}, {coord.z}]";

        PopulateVoxelMap();
        UpdataChunk();
        world.ChunkQueuePush(this);
        int[,] arr = new int[4, 2]
        {
            { 1, 0 },
            {-1, 0 },
            { 0, 1 },
            { 0,-1 }
        };
        for (int i = 0; i < 4; ++i)
        {
            Chunk chunk = world.GetChunk(new Vector2Int(coord.x + arr[i, 0], coord.z + arr[i, 1]));
            if(chunk != null)
            {
                world.ChunkQueuePush(chunk);
            }
        }
    }
    public void UpdataChunk()
    {
        ClearMeshData();
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    UpdateChunkData(new Vector3Int(x, y, z));
                }
            }
        }
        CreateMesh();
    }
    private void CreateMesh()
    {
        // �޽ÿ� �����͵� �ʱ�ȭ
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uv.ToArray();

        // ������� ����
        meshFilter.mesh.RecalculateNormals();
        //meshFilter.mesh.RecalculateBounds();
        //meshFilter.mesh.RecalculateTangents();
    }

    public void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        meshFilter.mesh.Clear();
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    if (y == 0)
                    {
                        voxelMap[x, y, z] = 2;
                        continue;
                    }
                    int height = GetPerlinNoise((float)x, (float)z);
                    if (height < y)
                    {
                        voxelMap[x, y, z] = 0;
                    }
                    else if (height == y)
                    {
                        voxelMap[x, y, z] = 2;
                    }
                    else if ((height > y))
                    {
                        voxelMap[x, y, z] = 3;
                    }
                }
            }
        }
    }
    private void UpdateChunkData(Vector3Int pos)
    {
        if (GetBlockID(pos) == 0)
        {
            return;
        }
        // 6������ �� �׸���
        // p : -Z, +Z, +Y, -Y, -X, +X ������ �̷����, ť���� �� �鿡 ���� �ε���
        for (int face = 0; face < 6; ++face)
        {
            // Face Check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��)�� ���� �� 
            // Solid�� �ƴ� ��쿡�� ť���� ���� �׷������� �ϱ�
            // => ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ���
            if (false == CheckProximityVoxel(pos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4�� �߰�
                for (int i = 0; i < 4; ++i)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                ushort blockID = GetBlockID(pos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode);
                // 2. Triangle�� ���ؽ� �ε��� 6�� �߰�
                int[] arr = new int[] { 0, 1, 2, 2, 1, 3 };
                foreach (int i in arr)
                {
                    triangles.Add(vertexIndex + i);
                }
                vertexIndex += 4;
            }
        }
    }

    /// <summary> ������ ���� ���������� Ȯ���Ѵ�.</summary>
    private bool CheckProximityVoxel(Vector3Int pos)
    {
        if (pos.x < 0)
        {
            return CheckProximityChunk(
                new Vector3Int(VoxelData.ChunkWidth - 1, pos.y, pos.z),
                new Vector2Int(coord.x - 1, coord.z));
        }
        else if (pos.x > VoxelData.ChunkWidth - 1)
        {
            return CheckProximityChunk(
                new Vector3Int(0, pos.y, pos.z),
                new Vector2Int(coord.x + 1, coord.z));
        }
        else if (pos.z < 0)
        {
            return CheckProximityChunk(
                new Vector3Int(pos.x, pos.y, VoxelData.ChunkWidth - 1),
                new Vector2Int(coord.x, coord.z - 1));
        }
        else if (pos.z > VoxelData.ChunkWidth - 1)
        {
            return CheckProximityChunk(
                new Vector3Int(pos.x, pos.y, 0),
                new Vector2Int(coord.x, coord.z + 1));
        }
        else if (pos.y < 0 || pos.y > VoxelData.ChunkHeight - 1)
        {
            return false;
        }
        return GetBlockID(pos) != 0;
    }
    private bool CheckProximityChunk(Vector3Int blockPos, Vector2Int chunkPos)
    {
        Chunk chunk = world.GetChunk(chunkPos);
        int? blockCode = chunk?.GetBlockID(blockPos);
        if (blockCode == null)
        {
            return false;
        }
        return (blockCode != 0);
    }
    private void AddTextureUV(int textureID)
    {
        // ��Ʋ�� ���� �ؽ��� ����, ���� ����
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    // (x, y) : (0, 0) ������ ���ϴ�
    /// <summary> �ؽ��� ��Ʋ�� ������ (x, y) ��ġ�� �ؽ��� UV�� uvs ����Ʈ�� �߰� </summary>
    private void AddTextureUV(int x, int y)
    {
        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        uv.Add(new Vector2(uvX, uvY));
        uv.Add(new Vector2(uvX, uvY + nh));
        uv.Add(new Vector2(uvX + nw, uvY));
        uv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private ushort GetBlockID(in Vector3 pos)
    {
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }
    private int GetPerlinNoise(float x, float z)
    {
        x += (coord.x * 16);
        z += (coord.z * 16);
        int scale = 30;
        int minHeight = 20;
        float height = Mathf.PerlinNoise((float)(x / scale) + world.seed, (float)(z / scale) + world.seed);
        int heightValue = Mathf.RoundToInt(height * 10);
        return heightValue + minHeight;
    }
}
