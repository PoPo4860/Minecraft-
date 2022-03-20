using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private ushort[,,] voxelMap =
        new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public World world;


    #region 헬퍼
    private int[] vertexData = new int[] { 0, 1, 2, 2, 1, 3 };

    int[,] vectorCoord = new int[4,2] { { -1, 0 }, { +1, 0 }, { 0, -1 }, { 0, +1 } };
    private Vector2Int VectorCoord(int num) => new Vector2Int(coord.x + vectorCoord[num,0], coord.z + vectorCoord[num,1]);
    private Vector3Int VectorBlock(Vector3 voxelFacePos, int i)
    {
        switch(i)
        {
            case 0:
                return new Vector3Int(VoxelData.ChunkWidth - 1, (int)voxelFacePos.y, (int)voxelFacePos.z);
            case 1:
                return new Vector3Int(0, (int)voxelFacePos.y, (int)voxelFacePos.z);
            case 2:
                return new Vector3Int((int)voxelFacePos.x, (int)voxelFacePos.y, VoxelData.ChunkWidth - 1);
            default:
                return new Vector3Int((int)voxelFacePos.x, (int)voxelFacePos.y, 0);
        }
    }


    #endregion

    #region 메쉬데이터
    private int vertexIndex = 0;
    private List<Vector3>[,,] vertices = new List<Vector3>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private List<int>[,,] triangles = new List<int>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private List<Vector2>[,,] uv = new List<Vector2>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    List<Vector3> meshVertices = new List<Vector3>();
    List<int> meshTriangles = new List<int>();
    List<Vector2> meshUv = new List<Vector2>();
    #endregion

    #region 오브젝트 데이터
    public GameObject ChunkObject; // 청크가 생성될 대상 게임오브젝트
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    #endregion

    #region 청크 데이터
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
        // 메시에 데이터들 초기화
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
                    // p : -Z, +Z, +Y, -Y, -X, +X 순서로 이루어진, 큐브의 각 면에 대한 인덱스
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

        // 변경사항 적용
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
        // p : -Z, +Z, +Y, -Y, -X, +X 순서로 이루어진, 큐브의 각 면에 대한 인덱스
        for (int face = 0; face < 6; ++face)
        {
            if (0 == GetBlockID(voxelPos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4개 추가
                for (int i = 0; i < 4; ++i)
                {
                    vertices[voxelPos.x, voxelPos.y, voxelPos.z].Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + voxelPos);
                }

                ushort blockID = GetBlockID(voxelPos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode, voxelPos);
            }
        }
    }
    private void AddTextureUV(int textureID, Vector3Int voxelPos)
    {
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // 해당 텍스쳐의 uv를 LB-LT-RB-RT 순서로 추가
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX, uvY));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX, uvY + nh));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX + nw, uvY));
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Add(new Vector2(uvX + nw, uvY + nh));
    }
    public ushort GetBlockID(in Vector3 voxelPos)
    {
        if (voxelPos.x <= -1)
        {
            return CheckProximityChunk(VectorCoord(0)).GetBlockID(VectorBlock(voxelPos, 0));
        }
        else if (voxelPos.x >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(VectorCoord(1)).GetBlockID(VectorBlock(voxelPos, 1));
        }
        else if (voxelPos.z <= -1)
        {
            return CheckProximityChunk(VectorCoord(2)).GetBlockID(VectorBlock(voxelPos, 2));
        }
        else if (voxelPos.z >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(VectorCoord(3)).GetBlockID(VectorBlock(voxelPos, 3));
        };
        if (voxelPos.y <= -1 || voxelPos.y >= VoxelData.ChunkHeight)
        {
            return 0;
        }
        return voxelMap[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z];
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
                CheckProximityChunk(VectorCoord(0)).ClearBolckData(VectorBlock(voxelPos, 0));
            }
            else if (voxelFacePos.x >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(1)).ClearBolckData(VectorBlock(voxelPos, 1));
            }
            else if (voxelFacePos.z <= -1)
            {
                CheckProximityChunk(VectorCoord(2)).ClearBolckData(VectorBlock(voxelPos, 2));
            }
            else if (voxelFacePos.z >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(3)).ClearBolckData(VectorBlock(voxelPos, 3));
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
                CheckProximityChunk(VectorCoord(0)).CreateMeshkData(VectorBlock(voxelPos, 0));
            }
            else if (voxelFacePos.x >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(1)).CreateMeshkData(VectorBlock(voxelPos, 1));
            }
            else if (voxelFacePos.z <= -1)
            {
                CheckProximityChunk(VectorCoord(2)).CreateMeshkData(VectorBlock(voxelPos, 2));
            }
            else if (voxelFacePos.z >= VoxelData.ChunkWidth)
            {
                CheckProximityChunk(VectorCoord(3)).CreateMeshkData(VectorBlock(voxelPos, 3));
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
