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

    private GameObject chunkObject; // 청크가 생성될 대상 게임오브젝트
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
        // 메시에 데이터들 초기화
        Mesh mesh = new Mesh
        {
            //List<type>.ToArray(); 메서드는 List의 요소를 새 배열로 복사한다.
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        // 변경사항 적용
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
        // 6방향의 면 그리기
        // p : -Z, +Z, +Y, -Y, -X, +X 순서로 이루어진, 큐브의 각 면에 대한 인덱스
        for (int face = 0; face < 6; ++face)
        {
            // Face Check(면이 바라보는 방향으로 +1 이동하여 확인)를 했을 때 
            // Solid가 아닌 경우에만 큐브의 면이 그려지도록 하기
            // => 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록
            if (CheckVoxel(pos) && !CheckVoxel(pos + VoxelData.faceChecks[face]))
            {
                // 1. Vertex, UV 4개 추가
                for (int i = 0; i <= 3; i++)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                byte blockID = GetBlockID(pos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode);
                // 2. Triangle의 버텍스 인덱스 6개 추가
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

        // 맵 범위를 벗어나는 경우
        if (x < 0 || x > VoxelData.ChunkWidth - 1 ||
           y < 0 || y > VoxelData.ChunkHeight - 1 ||
           z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;

        return voxelMap[x, y, z] != 0;
    }
    private void AddTextureUV(int textureID)
    {
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);
        
        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    // (x, y) : (0, 0) 기준은 좌하단
    /// <summary> 텍스쳐 아틀라스 내에서 (x, y) 위치의 텍스쳐 UV를 uvs 리스트에 추가 </summary>
    private void AddTextureUV(int x, int y)
    {
        if (x < 0 || y < 0 || x >= VoxelData.TextureAtlasWidth || y >= VoxelData.TextureAtlasHeight)
        {
            throw new System.IndexOutOfRangeException($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");
        }

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // 해당 텍스쳐의 uv를 LB-LT-RB-RT 순서로 추가
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
