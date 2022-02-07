using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk
{
    //public MeshRenderer meshRenderer;
    //public MeshFilter meshFilter;

    private byte[,,] voxelMap = 
        new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
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
    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
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
                    voxelMap[x, y, z] = 2;
                }
            }
        }
    }
    private void AddVoxelDataToChunk(Vector3 pos)
    {
        // 6������ �� �׸���
        // p : -Z, +Z, +Y, -Y, -X, +X ������ �̷����, ť���� �� �鿡 ���� �ε���
        for (int face = 0; face < 6; ++face)
        {
            // Face Check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��)�� ���� �� 
            // Solid�� �ƴ� ��쿡�� ť���� ���� �׷������� �ϱ�
            // => ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ���
            if (CheckVoxel(pos) && !CheckVoxel(pos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4�� �߰�
                for (int i = 0; i <= 3; i++)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                byte blockID = GetBlockID(pos);
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

    private bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        // �� ������ ����� ���
        if (x < 0 || x > VoxelData.ChunkWidth - 1 ||
           y < 0 || y > VoxelData.ChunkHeight - 1 ||
           z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;

        return voxelMap[x, y, z] != 0;
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
    private byte GetBlockID(in Vector3 pos)
    {
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }
}
