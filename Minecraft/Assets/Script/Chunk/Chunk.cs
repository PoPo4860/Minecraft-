using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private readonly  ushort[,,] voxelMap =
        new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private readonly World world;

    private readonly Queue<Vector3Int> createOrePos = new Queue<Vector3Int>();
    private readonly Queue<Vector3Int> createTreePos = new Queue<Vector3Int>();

    #region 메쉬데이터
    private int vertexIndex = 0;
    private readonly List<Vector3>[,,] vertices = new List<Vector3>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private readonly List<int>[,,] triangles = new List<int>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    //private List<int>[,,] transparencyTriangles = new List<int>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private readonly List<Vector2>[,,] uv = new List<Vector2>[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private readonly List<Color> colors = new List<Color>();

    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    //List<int> meshtransparencyTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();
    #endregion

    #region 오브젝트 데이터
    public readonly GameObject ChunkObject; // 청크가 생성될 대상 게임오브젝트
    private readonly MeshRenderer meshRenderer;
    private readonly MeshFilter meshFilter;
    //private Material[] materials = new Material[2];
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
                    //transparencyTriangles[x, y, z] = new List<int>();
                    uv[x, y, z] = new List<Vector2>();
                }
            }
        }

        //materials[0] = world.TextureAtlas;
        //materials[1] = world.TextureAtlasTrans;
        meshRenderer.material = world.TextureAtlas;
        ChunkObject.transform.SetParent(world.transform);
        ChunkObject.transform.position =
            new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        ChunkObject.name = $"Chunk [{coord.x}, {coord.z}]";

        PopulateChunkMap();
        ChunkObject.SetActive(false);
    }
    public IEnumerator CreateChunkMesh()
    {
        chunkState = ChunkState.CoroutineUpdate;
        ChunkObject.SetActive(true);
        for (int i = 0; i < 4; ++i)
        {
            if (null == world.GetChunkFromCoord(ChunkHelperData.VectorCoord(i, coord)))
            {
                world.CreateNewChunk(ChunkHelperData.VectorCoord(i, coord));
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
        colors.Clear();
        //meshtransparencyTriangles.Clear();
        meshUv.Clear();
        vertexIndex = 0;


        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    triangles[x, y, z].Clear();
                    //transparencyTriangles[x, y, z].Clear();

                    Vector3Int voxelPos = new Vector3Int(x, y, z);
                    if (0 == GetBlockID(voxelPos))
                        continue;

                    int yPos = y + 1;
                    bool inShade = false;
                    while (yPos < VoxelData.ChunkHeight)
                    {
                        int blockCode = GetBlockID(new Vector3(x, yPos, z));
                        if (CodeData.BLOCK_AIR != blockCode && CodeData.BLOCK_LEAF != blockCode)
                        {
                            inShade = true;
                            break;
                        }
                        ++yPos;
                    }

                    //bool isTrans = (CodeData.BLOCK_LEAF == GetBlockID(voxelPos)) ? true : false;
                    for (int face = 0; face < 6; ++face)
                    {
                        int blockCode = GetBlockID(voxelPos + VoxelData.faceChecks[face]);
                        if (CodeData.BLOCK_LEAF == blockCode || CodeData.BLOCK_AIR == blockCode)
                        {

                            foreach (int i in ChunkHelperData.vertexData)
                            {
                                triangles[voxelPos.x, voxelPos.y, voxelPos.z].Add(vertexIndex + i);
                                //if (isTrans)
                                //    transparencyTriangles[voxelPos.x, voxelPos.y, voxelPos.z].Add(vertexIndex + i);
                                //else
                                //    triangles[voxelPos.x, voxelPos.y, voxelPos.z].Add(vertexIndex + i);
                            }
                            vertexIndex += 4;



                            float lightLevel;
                            if (inShade)
                                lightLevel = 0.4f;
                            else
                                lightLevel = 0f;

                            colors.Add(new Color(0, 0, 0, lightLevel));
                            colors.Add(new Color(0, 0, 0, lightLevel));
                            colors.Add(new Color(0, 0, 0, lightLevel));
                            colors.Add(new Color(0, 0, 0, lightLevel));
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
                        continue;

                    meshVertices.AddRange(vertices[x, y, z]);
                    meshTriangles.AddRange(triangles[x, y, z]);
                    //meshtransparencyTriangles.AddRange(transparencyTriangles[x, y, z]);
                    meshUv.AddRange(uv[x, y, z]);
                }
            }
        }
        meshFilter.mesh.vertices = meshVertices.ToArray();
        //meshFilter.mesh.subMeshCount = 2;
        //meshFilter.mesh.SetTriangles(meshTriangles.ToArray(), 0);
        //meshFilter.mesh.SetTriangles(meshtransparencyTriangles.ToArray(), 1);
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUv.ToArray();
        meshFilter.mesh.colors = colors.ToArray();
        meshFilter.mesh.RecalculateNormals();
    }
    private void PopulateChunkMap()
    {
        for (int y = VoxelData.ChunkHeight-1; y > 0; --y)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; ++z)
                {
                    voxelMap[x, y, z] = PopulateBlock(x, y, z);
                }
            }
        }
        SetOre();
        SetTree();
    }
    private ushort PopulateBlock(in int x, in int y, in int z)
    {
        if (60 < y)
            return CodeData.BLOCK_AIR;
        if (1 == y)
            return CodeData.BLOCK_BEDROCK;

        float terrainResult = Perlin.GetPerlinNoiseTerrain(coord, world.worldSeed, x, y, z);
        if (terrainResult < 0)
            return CodeData.BLOCK_AIR;
        
        float cavePerlin = Perlin.GetPerlinNoiseCave(coord, world.worldSeed, x, y, z);
        if (0.499f < cavePerlin && cavePerlin < 0.5f)
            createOrePos.Enqueue(new Vector3Int(x, y, z));

        if (Perlin.GetPerlinNoiseCave(coord, world.worldSeed, x, y, z) < 0.5f)
            return CodeData.BLOCK_AIR;

        if (2 < terrainResult)
            return CodeData.BLOCK_STONE;
        
        if (0 == voxelMap[x, y + 1, z])
        {
            if (0 == Random.Range(0, 100))
                createTreePos.Enqueue(new Vector3Int(x, y + 1, z));
            return CodeData.BLOCK_GRASS;
        }
        
        return CodeData.BLOCK_DIRT;
    }
    private void SetOre()
    {
        while(0 != createOrePos.Count)
        {
            Vector3Int basePos = createOrePos.Peek();
            createOrePos.Dequeue();
            
            if (20 <= basePos.y && 0 == Random.Range(0, 2))
                SetOre(basePos, CodeData.BLOCK_COAL, 2);
            
            else if (5 <= basePos.y && basePos.y < 30 && 0 == Random.Range(0, 4))
                SetOre(basePos, CodeData.BLOCK_IRON, 3);
         
            else if (1 < basePos.y && basePos.y < 10 && 0 == Random.Range(0, 5))
                SetOre(basePos, CodeData.BLOCK_DIAMOND, 7);
        }
    }
    private void SetOre(in Vector3Int basePos, in ushort blockCode,in int randomValue)
    {
        voxelMap[basePos.x, basePos.y, basePos.z] = blockCode;
        foreach (Vector3Int pos in ChunkHelperData.oreProximityPos)
        {
            int posX = basePos.x + pos.x;
            int posY = basePos.y + pos.y;
            int posZ = basePos.z + pos.z;
            if (posX < 0 || posY < 2 || posZ < 0)
                continue;
           
            if (VoxelData.ChunkWidth - 1 < posX || VoxelData.ChunkHeight - 1 < posY || VoxelData.ChunkWidth - 1 < posZ)
                continue;
           
            if (voxelMap[posX, posY, posZ] == CodeData.BLOCK_STONE && 0 == Random.Range(0, randomValue))
                voxelMap[posX, posY, posZ] = blockCode;
        }
    }
    private void SetTree()
    {
        while(0 != createTreePos.Count)
        {
            Vector3Int pos = createTreePos.Peek();
            createTreePos.Dequeue();

            if (pos.x < 2 || VoxelData.ChunkWidth - 3 < pos.x)
                continue;
            if (pos.z < 2 || VoxelData.ChunkWidth - 3 < pos.z)
                continue;

            bool isCanTree = true;
            for (int y = 0; y < 6; ++y)
            {
                for (int i = 0; i < 25; ++i)
                {
                    if (-1 == ChunkHelperData.treePos[y, i])
                        continue;

                    int posX = pos.x + ((i % 5) - 2);
                    int posY = pos.y + y;
                    int posZ = pos.z + ((i / 5) - 2);
                    int blockCode = GetBlockID(new Vector3(posX, posY, posZ));
                    if (CodeData.BLOCK_AIR != blockCode && CodeData.BLOCK_LEAF != blockCode)
                    {
                        isCanTree = false;
                        break;
                    }
                }
                if (false == isCanTree)
                    break;
            }

            if(false == isCanTree)
                continue;

            for (int y = 0; y < 6; ++y)
            {
                for (int i = 0; i < 25; ++i)
                {
                    int posX = pos.x + ((i % 5) - 2);
                    int posY = pos.y + y;
                    int posZ = pos.z + ((i / 5) - 2);
                    if (-1 != ChunkHelperData.treePos[y, i])
                        voxelMap[posX, posY, posZ] = (ushort)ChunkHelperData.treePos[y, i];
                }
            }
        }
    }
    private void CreateMeshkData(in Vector3Int voxelPos)
    {
        if (GetBlockID(voxelPos) == 0)
            return;

        for (int face = 0; face < 6; ++face)
        {
            int faceBlockCode = GetBlockID(voxelPos + VoxelData.faceChecks[face]);
            if (CodeData.BLOCK_AIR == faceBlockCode || CodeData.BLOCK_LEAF == faceBlockCode)
            {
                for (int i = 0; i < 4; ++i)
                    vertices[voxelPos.x, voxelPos.y, voxelPos.z].Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + voxelPos);

                ushort blockID = GetBlockID(voxelPos);
                int atlasesCode = CodeData.GetBlockTextureAtlases(blockID, face);
                AddTextureUV(atlasesCode, voxelPos);
            }
        }
    }
    private void AddTextureUV(int atlasesCode, Vector3Int voxelPos)
    {
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = atlasesCode % w;
        int y = h - (atlasesCode / w) - 1;

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
        for(int i = 0; i < 4; ++i)
        {
            if(ChunkHelperData.Asd(voxelPos, i))
                return CheckProximityChunk(ChunkHelperData.VectorCoord(i, coord)).GetBlockID(ChunkHelperData.VectorBlock(voxelPos, i));
        }

        if (ChunkHelperData.Asd(voxelPos, 4))
            return 0;

        return voxelMap[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z];
    }
    private Chunk CheckProximityChunk(Vector2Int chunkPos)
    {
        Chunk chunk = world.GetChunkFromCoord(chunkPos);
        if (chunk == null)
        {
            return null;
        }
        return chunk;
    }
    private void ClearBolckData(in Vector3Int voxelPos)
    {
        if (0 == GetBlockID(voxelPos))
            return;
        vertices[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
        //transparencyTriangles[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
        triangles[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
        uv[voxelPos.x, voxelPos.y, voxelPos.z].Clear();
    }
    public void ModifyChunkData(in Vector3Int voxelPos, ushort blockCode)
    {
        for (int face = 0; face < 6; ++face)
        {
            Vector3Int voxelFacePos = voxelPos + VoxelData.faceChecks[face];
            for (int i = 0; i < 5; ++i)
            {
                if(i == 4 && false == ChunkHelperData.Asd(voxelFacePos, 4))
                {
                    ClearBolckData(voxelFacePos);
                    break;
                }
                if (ChunkHelperData.Asd(voxelFacePos, i))
                {
                    CheckProximityChunk(ChunkHelperData.VectorCoord(i, coord)).ClearBolckData(ChunkHelperData.VectorBlock(voxelPos, i));
                    break;
                }
            }
        }

        if (blockCode != CodeData.BLOCK_AIR)
            voxelMap[voxelPos.x, voxelPos.y, voxelPos.z] = blockCode;
        else
        {
            ClearBolckData(voxelPos);
            voxelMap[voxelPos.x, voxelPos.y, voxelPos.z] = 0;
        }

        CreateMeshkData(voxelPos);
        for (int face = 0; face < 6; ++face)
        {
            Vector3Int voxelFacePos = voxelPos + VoxelData.faceChecks[face];
            for (int i = 0; i < 5; ++i)
            {
                if (i == 4 && false == ChunkHelperData.Asd(voxelFacePos, 4))
                {
                    CreateMeshkData(voxelFacePos);
                    break;
                }

                if (ChunkHelperData.Asd(voxelFacePos, i))
                {
                    CheckProximityChunk(ChunkHelperData.VectorCoord(i, coord)).CreateMeshkData(ChunkHelperData.VectorBlock(voxelPos, i));
                    break;
                }
            }
        }

        ApplyMeshData();

        if (voxelPos.x == 0)
            CheckProximityChunk(ChunkHelperData.VectorCoord(0, coord)).ApplyMeshData();
        else if (voxelPos.x == 15)
            CheckProximityChunk(ChunkHelperData.VectorCoord(1, coord)).ApplyMeshData();
        if (voxelPos.z == 0)
            CheckProximityChunk(ChunkHelperData.VectorCoord(2, coord)).ApplyMeshData();
        else if (voxelPos.z == 15)
            CheckProximityChunk(ChunkHelperData.VectorCoord(3, coord)).ApplyMeshData();
    }
}
