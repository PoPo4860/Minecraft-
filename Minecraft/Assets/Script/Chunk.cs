using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk
{
    private ushort[,,] voxelMap = 
        new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public World world;
    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private GameObject chunkObject; // ûũ�� ������ ��� ���ӿ�����Ʈ
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public ChunkCoord coord;
    public Chunk(ChunkCoord coord, World world)
    {
        this.coord = coord;
        this.world = world;

        chunkObject = new GameObject();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position =
            new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = $"Chunk [{coord.x}, {coord.z}]";

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }
    public void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    AddVoxelDataToChunk(new Vector3Int(x, y, z));
                }
            }
        }
    }
    private void CreateMesh()
    {
        // �޽ÿ� �����͵� �ʱ�ȭ
        Mesh mesh = new Mesh
        {
            //List<type>.ToArray(); �޼���� List�� ��Ҹ� �� �迭�� �����Ѵ�.
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        // ������� ����
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    if (y == VoxelData.ChunkHeight - 1) 
                    {
                        voxelMap[x, y, z] = 2;
                    }
                    else
                    {
                        voxelMap[x, y, z] = 3;
                    }
                }
            }
        }
    }
    private void AddVoxelDataToChunk(Vector3Int pos)
    {
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
                for (int i = 0; i <= 3; i++)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                ushort blockID = GetBlockID(pos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode);
                // 2. Triangle�� ���ؽ� �ε��� 6�� �߰�
                int[] arr = new int[]{ 0, 1, 2, 2, 1, 3 };
                foreach(int i in arr)
                {
                    triangles.Add(vertexIndex + i);
                }
                vertexIndex += 4;
            }
        }
    }

    /// <summary>
    /// ������ ���� ���������� Ȯ���Ѵ�.
    /// </summary>
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
            return CheckProximityChunk
                (new Vector3Int(pos.x, pos.y, 0), 
                new Vector2Int(coord.x, coord.z + 1));
        }
        else if (pos.y < 0 || pos.y > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        //if (x < 0 || x > VoxelData.ChunkWidth - 1 ||
        //   y < 0 || y > VoxelData.ChunkHeight - 1 ||
        //   z < 0 || z > VoxelData.ChunkWidth - 1)
        //{
        //    return false;
        //}
        return voxelMap[pos.x, pos.y, pos.z] != 0;
    }
    private bool CheckProximityChunk(Vector3Int blockPos, Vector2Int chunkPos)
    {
        int? blockCode = world.GetChunk(chunkPos)?.voxelMap[blockPos.x, blockPos.y, blockPos.z];
        if(blockCode == null)
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
        if (x < 0 || y < 0 || x >= VoxelData.TextureAtlasWidth || y >= VoxelData.TextureAtlasHeight)
        {
            throw new System.IndexOutOfRangeException($"�ؽ��� ��Ʋ���� ������ ������ϴ� : [x = {x}, y = {y}]");
        }

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX, uvY + nh));
        uvs.Add(new Vector2(uvX + nw, uvY));
        uvs.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private ushort GetBlockID(in Vector3 pos)
    {
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }
}
