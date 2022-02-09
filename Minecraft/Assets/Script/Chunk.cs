using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// 자기소개
// 저희 회사에 대해선?
// 게임 플레이 해봤냐? 어떤점이 감명깊었냐
// 대학에선 뭐했냐
// push_back과 emplace_back 의 차이;
// set과 map 구현 해봤는지
// 트리구조에 대해 설명해봐라
// 인터페이스와 추상클래스의 차이가 뭐냐, 인터페이스를 왜 사용하냐
// 왜 게임프로그래머가 되고싶냐
// 협업하면서 어려웠던것은 어떤점이냐
// 어떻게 해결했냐
// struct와 class랑 어떤 차이가 있냐
// 값타입과 참조타입엔 무슨 차이가 있냐
// C# 에서 가비지컬렉션이 어떻게 작동되는가
// 가비지 컬렉션에서 루트를 참조한다는게 무슨 뜻이냐
// 유니티에선 가비지컬렉션이 동작하냐
// 박싱과 언박싱의 차이점이 뭐냐
// 코루틴에 대해서 설명해라
// 코루틴이 내부적으로 어떻게 동작하냐
// 코루틴을 구현해봤냐? 안했으면 어떻게 구현해볼 것이냐
// Tramsfprm과 RectTransform의 차이가 뭐냐
// 전략패턴이란 뭐냐
// 니가 만든 프로젝트에 이 시스템이 있던데, 이건 어떻게 구현을 했엇나
// A* 알고리즘이 어떻게 동작하냐? 순서를 설명해달라
// 니가 만든 프로젝트에서 이 코드가있던데, 이렇게 구현한 이유가 있나
// 유니티의 MonoBehaviour의 동작과정을 설명해달라
// MonoBehaviour 에서 FixedUpdate()와 Update()와 LasteUpdate() 가 있다고 말했는데. 이 셋의 차이점이 무엇이냐
// ScriptableObject 공부
//
// 마지막으로 앞으로 어떤 프로그래머가 되고싶은가?
// 회사에 대해 물어볼 것이 있는가?
//
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
        if(a.x == b.x && a.z == b.z)
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
    private List<Vector2> uvs = new List<Vector2>();

    public GameObject ChunkObject; // 청크가 생성될 대상 게임오브젝트
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
                for (int i = 0; i <= 3; i++)
                {
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + pos);
                }

                ushort blockID = GetBlockID(pos);
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

    /// <summary>
    /// 인접한 블럭의 존재유무를 확인한다.
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
            throw new System.IndexOutOfRangeException($"텍스쳐 아틀라스의 범위를 벗어났을지도? : [x = {x}, y = {y}]");
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
    private ushort GetBlockID(in Vector3 pos)
    {
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }
}
