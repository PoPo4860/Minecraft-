using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private ushort[,,] voxelMap =
        new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public World world;


    #region ����
    private readonly int[,] cerateChunkPos = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
    private int[] vertexData = new int[] { 0, 1, 2, 2, 1, 3 };

    int[,] numbers = { { -1, 0 }, { +1, 0 }, { 0, -1 }, { 0, +1 } };
    private Vector2Int VectorCoord(int num) => new Vector2Int(coord.x + numbers[num,0], coord.z + numbers[num,1]);
    private Vector3Int vectorCoordd(Vector3Int voxelFacePos) => new Vector3Int(VoxelData.ChunkWidth - 1, voxelFacePos.y, voxelFacePos.z);

    #endregion

    #region �޽�������
    private int vertexIndex = 0;
    private List<Vector3>[,,] vertices = new List<Vector3>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private List<int>[,,] triangles = new List<int>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private List<Vector2>[,,] uv = new List<Vector2>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    List<Vector3> meshVertices = new List<Vector3>();
    List<int> meshTriangles = new List<int>();
    List<Vector2> meshUv = new List<Vector2>();
    #endregion

    #region ������Ʈ ������
    public GameObject ChunkObject; // ûũ�� ������ ��� ���ӿ�����Ʈ
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    #endregion

    #region ûũ ������
    public enum ChunkState { CoroutineStart, CoroutineUpdate, CoroutineEnd };
    public ChunkState chunkState = ChunkState.CoroutineStart;
    public ChunkCoord coord;
    #endregion

    public Chunk(ChunkCoord coord, World world)
    {
        this.coord = coord;
        this.world = world;
        ChunkObject = new GameObject();
        meshRenderer = ChunkObject.AddComponent<MeshRenderer>();
        meshFilter = ChunkObject.AddComponent<MeshFilter>();

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    vertices[x, y, z] = new List<Vector3>();
                    triangles[x, y, z] = new List<int>();
                    uv[x, y, z] = new List<Vector2>();
                }
            }
        }

        meshRenderer.material = world.Atlas;
        ChunkObject.transform.SetParent(world.transform);
        ChunkObject.transform.position =
            new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        ChunkObject.name = $"Chunk [{coord.x}, {coord.z}]";

        PopulateVoxelMap();
        ChunkObject.SetActive(false);
    }
    public IEnumerator CreateChunkMesh()
    {
        chunkState = ChunkState.CoroutineUpdate;
        ChunkObject.SetActive(true);
        for (int i = 0; i < 4; ++i)
        {
            if (null == world.GetChunk(VectorCoord(i)))
            {
                world.CreateNewChunk(VectorCoord(i));
            }
        }

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
#if (DEBUG_MODE)
            if (false) yield return null;
#else
            if (y % 5 == 0) yield return null;
#endif
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    CreateMeshkData(new Vector3Int(x, y, z));
                }
            }
        }

        ApplyMeshData();
        chunkState = ChunkState.CoroutineEnd;
    }
    private void ApplyMeshData()
    {
        // �޽ÿ� �����͵� �ʱ�ȭ
        meshFilter.mesh.Clear();
        meshVertices.Clear();
        meshTriangles.Clear();
        meshUv.Clear();
        vertexIndex = 0;
        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    Vector3Int voxelPos = new Vector3Int(x, y, z);
                    triangles[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
                    if (0 == GetBlockID(voxelPos))
                    {
                        continue;
                    }
                    // p : -Z, +Z, +Y, -Y, -X, +X ������ �̷����, ť���� �� �鿡 ���� �ε���
                    for (int face = 0; face < 6; ++face)
                    {
                        if (0 == GetBlockID(voxelPos + VoxelData.faceChecks[face]))
                        {
                            foreach (int i in vertexData)
                            {
                                triangles[voxelPos.x, voxelPos.y, voxelPos.z].Add(vertexIndex + i);
                            }
                            vertexIndex += 4;
                        }
                    }
                }
            }
        }

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    if (0 == GetBlockID(new Vector3(x, y, z)))
                    {
                        continue;
                    }
                    meshVertices.AddRange(vertices[x, y, z]);
                    meshTriangles.AddRange(triangles[x, y, z]);
                    meshUv.AddRange(uv[x, y, z]);
                }
            }
        }
        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUv.ToArray();

        // ������� ����
        meshFilter.mesh.RecalculateNormals();
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
                    int height = Perlin.GetPerlinNoise(coord, world.seed, x, z);
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
    private void CreateMeshkData(Vector3Int voxelPos)
    {
        if (GetBlockID(voxelPos) == 0)
        {
            return;
        }
        // p : -Z, +Z, +Y, -Y, -X, +X ������ �̷����, ť���� �� �鿡 ���� �ε���
        for (int face = 0; face < 6; ++face)
        {
            if (0 == GetBlockID(voxelPos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4�� �߰�
                for (int i = 0; i < 4; ++i)
                {
                    vertices[voxelPos.x, voxelPos.y, voxelPos.z].Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + voxelPos);
                }

                ushort blockID = GetBlockID(voxelPos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode, voxelPos);
                //foreach (int i in vertexData)
                //{
                //    triangles[voxelPos.x, voxelPos.y, voxelPos.z].Add(vertexIndex + i);
                //}
                //vertexIndex += 4;
            }
        }
    }
    private void AddTextureUV(int textureID, Vector3Int voxelPos)
    {
        // ��Ʋ�� ���� �ؽ��� ����, ���� ����
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX, uvY));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX, uvY + nh));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX + nw, uvY));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX + nw, uvY + nh));
    }
    public ushort GetBlockID(in Vector3 pos)
    {
        if (pos.x <= -1)
        {
            return CheckProximityChunk(VectorCoord(0)).
               GetBlockID(new Vector3Int(VoxelData.ChunkWidth - 1, (int)pos.y, (int)pos.z));
        }
        else if (pos.x >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(VectorCoord(1)).
               GetBlockID(new Vector3Int(0, (int)pos.y, (int)pos.z));
        }
        else if (pos.z <= -1)
        {
            return CheckProximityChunk(VectorCoord(2)).
               GetBlockID(new Vector3Int((int)pos.x, (int)pos.y, VoxelData.ChunkWidth - 1));
        }
        else if (pos.z >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(VectorCoord(3)).
               GetBlockID(new Vector3Int((int)pos.x, (int)pos.y, 0));
        };
        if (pos.y <= -1 || pos.y >= VoxelData.ChunkHeight)
        {
            return 0;
        }
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }
    private Chunk CheckProximityChunk(Vector2Int chunkPos)
    {
        Chunk chunk = world.GetChunk(chunkPos);
        if (chunk == null)
        {
            return null;
        }
        return chunk;
    }
    private void ClearBolckData(in Vector3Int voxelPos)
    {
        if (0 == GetBlockID(voxelPos))
        {
            return;
        }
        vertices[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
        triangles[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
    }

    public void ModifyChunkData(in Vector3Int voxelPos, ushort blockCode)
    {
        for (int face = 0; face < 6; ++face)
        {
            Vector3Int voxelFacePos = voxelPos + VoxelData.faceChecks[face];
            if (voxelFacePos.x <= -1)
            {
                CheckProximityChunk(new Vector2Int(coord.x - 1, coord.z)).
                   ClearBolckData(new Vector3Int(VoxelData.ChunkWidth - 1, voxelFacePos.y, voxelFacePos.z));
            }
            else if (voxelFacePos.x >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(new Vector2Int(coord.x + 1, coord.z)).
                   ClearBolckData(new Vector3Int(0, voxelFacePos.y, voxelFacePos.z));
            }
            else if (voxelFacePos.z <= -1)
            {
                CheckProximityChunk(new Vector2Int(coord.x, coord.z - 1)).
                   ClearBolckData(new Vector3Int(voxelFacePos.x, voxelFacePos.y, VoxelData.ChunkWidth - 1));
            }
            else if (voxelFacePos.z >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(new Vector2Int(coord.x, coord.z + 1)).
                   ClearBolckData(new Vector3Int(voxelFacePos.x, voxelFacePos.y, 0));
            }
            else if (voxelFacePos.y <= -1 || voxelFacePos.y >= VoxelData.ChunkHeight) { }
            else
            {
                ClearBolckData(voxelFacePos);
            }
        }

        if (voxelMap[voxelPos.x, voxelPos.y, voxelPos.z] == 0)
        {
            voxelMap[voxelPos.x, voxelPos.y, voxelPos.z] = blockCode;
        }
        else
        {
            ClearBolckData(voxelPos);
            voxelMap[voxelPos.x, voxelPos.y, voxelPos.z] = 0;
        }

        CreateMeshkData(voxelPos);
        for (int face = 0; face < 6; ++face)
        {
            Vector3Int voxelFacePos = voxelPos + VoxelData.faceChecks[face];
            if (voxelFacePos.x <= -1)
            {
                CheckProximityChunk(VectorCoord(0)).
                   CreateMeshkData(new Vector3Int(VoxelData.ChunkWidth - 1, voxelFacePos.y, voxelFacePos.z));
            }
            else if (voxelFacePos.x >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(1)).
                   CreateMeshkData(new Vector3Int(0, voxelFacePos.y, voxelFacePos.z));
            }
            else if (voxelFacePos.z <= -1)
            {
                CheckProximityChunk(VectorCoord(2)).
                   CreateMeshkData(new Vector3Int(voxelFacePos.x, voxelFacePos.y, VoxelData.ChunkWidth - 1));
            }
            else if (voxelFacePos.z >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(3)).
                   CreateMeshkData(new Vector3Int(voxelFacePos.x, voxelFacePos.y, 0));
            }
            else if (voxelFacePos.y <= -1 || voxelFacePos.y >= VoxelData.ChunkHeight) { }
            else
            {
                CreateMeshkData(voxelFacePos);
            }
        }

        ApplyMeshData();

        if (voxelPos.x == 0)
        {
            CheckProximityChunk(VectorCoord(0)).ApplyMeshData();
        }
        else if (voxelPos.x == 15)
        {
            CheckProximityChunk(VectorCoord(1)).ApplyMeshData();
        }
        if (voxelPos.z == 0)
        {
            CheckProximityChunk(VectorCoord(2)).ApplyMeshData();
        }
        else if (voxelPos.z == 15)
        {
            CheckProximityChunk(VectorCoord(3)).ApplyMeshData();
        }
    }
}