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
        if ((a.x != b.x) || (a.z != b.z))
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

    #region 메쉬데이터
    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private int[,,] verticesIndexSize = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private List<int> triangles = new List<int>();
    private int[,,] trianglesIndexSize = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private List<Vector2> uv = new List<Vector2>();
    private int[,,] uvIndexSize = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    #endregion

    #region 오브젝트 데이터
    public GameObject ChunkObject; // 청크가 생성될 대상 게임오브젝트
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    #endregion

    /// <summary> 해당 청크가 생성중인 것인지 확인하는 bool. </summary>
    public bool chunkCoroutineIsRunning = false;

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
        //UpdataChunk();
        vertices.Capacity = 20000;
        triangles.Capacity = 30000;
        uv.Capacity = 20000;

        world.ChunkQueuePush(this);
    }
    //private unsafe void Asd()
    //{
    //    int* a = null;
    //    int* b = a;
    //}
    public IEnumerator UpdataChunk()
    {
        chunkCoroutineIsRunning = true;
        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            if (y % 5 == 0) yield return null;

            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    int verticesCount = vertices.Count;
                    int trianglesCount = triangles.Count;
                    int uvCount = uv.Count;

                    UpdateChunkData(new Vector3Int(x, y, z));

                    verticesIndexSize[x, y, z] = vertices.Count - verticesCount;
                    trianglesIndexSize[x, y, z] = triangles.Count - trianglesCount;
                    uvIndexSize[x, y, z] = uv.Count - uvCount;
                }
            }
        }

        CreateMesh();
        chunkCoroutineIsRunning = false;
    }
    private void CreateMesh()
    {
        //MesIterator asd = new MesIterator(vertices, uv, triangles, 1, 1, 1);

        // 메시에 데이터들 초기화
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uv.ToArray();

        // 변경사항 적용
        meshFilter.mesh.RecalculateNormals();
    }

    public void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
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
        // 6방향의 면 그리기
        // p : -Z, +Z, +Y, -Y, -X, +X 순서로 이루어진, 큐브의 각 면에 대한 인덱스
        for (int face = 0; face < 6; ++face)
        {
            // Face Check(면이 바라보는 방향으로 +1 이동하여 확인)를 했을 때 
            // Solid가 아닌 경우에만 큐브의 면이 그려지도록 하기
            // => 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록
            if (false == CheckProximityVoxel(pos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4개 추가
                for (int i = 0; i < 4; ++i)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                ushort blockID = GetBlockID(pos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode);
                // 2. Triangle의 버텍스 인덱스 6개 추가
                int[] arr = new int[] { 0, 1, 2, 2, 1, 3 };
                foreach (int i in arr)
                {
                    triangles.Add(vertexIndex + i);
                }
                vertexIndex += 4;
            }
        }
    }

    /// <summary> 인접한 블럭의 존재유무를 확인한다.</summary>
    private bool CheckProximityVoxel(Vector3Int pos)
    {
        if (pos.x < 0)
        {
            return 0 != CheckProximityChunk(
                new Vector3Int(VoxelData.ChunkWidth - 1, pos.y, pos.z),
                new Vector2Int(coord.x - 1, coord.z));
        }
        else if (pos.x > VoxelData.ChunkWidth - 1)
        {
            return 0 != CheckProximityChunk(
                new Vector3Int(0, pos.y, pos.z),
                new Vector2Int(coord.x + 1, coord.z));
        }
        else if (pos.z < 0)
        {
            return 0 != CheckProximityChunk(
                new Vector3Int(pos.x, pos.y, VoxelData.ChunkWidth - 1),
                new Vector2Int(coord.x, coord.z - 1));
        }
        else if (pos.z > VoxelData.ChunkWidth - 1)
        {
            return 0 != CheckProximityChunk(
                new Vector3Int(pos.x, pos.y, 0),
                new Vector2Int(coord.x, coord.z + 1));
        }
        else if (pos.y < 0 || pos.y > VoxelData.ChunkHeight - 1)
        {
            return false;
        }
        return 0 != GetBlockID(pos);
    }
    private ushort CheckProximityChunk(Vector3Int blockPos, Vector2Int chunkPos)
    {
        Chunk chunk = world.GetChunk(chunkPos);
        ushort? blockCode = chunk?.GetBlockID(blockPos);
        if (blockCode == null)
        {
            return 0;
        }
        return (ushort)blockCode; ;
    }
    private void AddTextureUV(int textureID)
    {
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    /// <summary> 텍스쳐 아틀라스 내에서 (x, y) 위치의 텍스쳐 UV를 uvs 리스트에 추가 </summary>
    private void AddTextureUV(int x, int y)
    {
        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // 해당 텍스쳐의 uv를 LB-LT-RB-RT 순서로 추가
        uv.Add(new Vector2(uvX, uvY));
        uv.Add(new Vector2(uvX, uvY + nh));
        uv.Add(new Vector2(uvX + nw, uvY));
        uv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    public ushort GetBlockID(in Vector3 pos)
    {
        if (pos.x <= -1)
        {
            return CheckProximityChunk(
                new Vector3Int(VoxelData.ChunkWidth - 1, (int)pos.y, (int)pos.z),
                new Vector2Int(coord.x - 1, coord.z));
        }
        else if (pos.x >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(
                new Vector3Int(0, (int)pos.y, (int)pos.z),
                new Vector2Int(coord.x + 1, coord.z));
        }
        else if (pos.z <= -1)
        {
            return CheckProximityChunk(
                new Vector3Int((int)pos.x, (int)pos.y, VoxelData.ChunkWidth - 1),
                new Vector2Int(coord.x, coord.z - 1));
        }
        else if (pos.z >= VoxelData.ChunkWidth)
        {
            return CheckProximityChunk(
               new Vector3Int((int)pos.x, (int)pos.y, 0),
               new Vector2Int(coord.x, coord.z + 1));
        };

        if (pos.y <= -1 || pos.y >= VoxelData.ChunkHeight)
        {
            return 0;
        }
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
