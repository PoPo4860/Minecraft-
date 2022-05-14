using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public VoxelRigidbody rigi;
    [HideInInspector] public int itemCode;
    [HideInInspector] public int itemNum;
    #region 메쉬데이터
    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;
    #endregion 

    public bool _canIsItemGet = false;
    public bool canIsItemGet
    {
        get { return (true == _canIsItemGet && true == gameObject.activeSelf); }
        set { _canIsItemGet = value; }
    }
    void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = World.Instance.TextureAtlas;
    }
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0));
    }
    private void OnEnable()
    {
        SetItemRender(itemCode);
        StartCoroutine(CanIsItemGetDelay());
        StartCoroutine(ActiveFalse());
        rigi.SetVelocity(Vector3.zero);
        rigi.SetForce(Vector3.zero);

    }
    private void OnDisable()
    {
        StopCoroutine(CanIsItemGetDelay());
        StopCoroutine(ActiveFalse());
        ClearItemRender();
        _canIsItemGet = false;

    }
    private void SetItemRender(int itemCode)
    {
        Vector3 vec = new Vector3(-0.5f, +0.5f, -0.5f);
        if(null != CodeData.GetBlockInfo(itemCode))
        {
            for (int face = 0; face < 6; ++face)
            {
                for (int i = 0; i < 4; ++i)
                    meshVertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + vec);

                foreach (int i in ChunkHelperData.vertexData)
                    meshTriangles.Add(vertexIndex + i);
                int atlasesCode = CodeData.GetBlockTextureAtlases(itemCode, face);

                AddBlockTextureUV(atlasesCode);
                vertexIndex += 4;
            }
        }
        else
        {
            for (int face = 0; face < 2; ++face)
            {
                for (int i = 0; i < 4; ++i)
                    meshVertices.Add(VoxelData.itemVerts[VoxelData.voxelTris[face, i]] + vec);

                foreach (int i in ChunkHelperData.vertexData)
                    meshTriangles.Add(vertexIndex + i);
                int atlasesCode = CodeData.GetItemTextureAtlases(itemCode);

                AddItemTextureUV(atlasesCode);
                vertexIndex += 4;
            }
        }

        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUv.ToArray();
        meshFilter.mesh.RecalculateNormals();
    }
    private void ClearItemRender()
    {
        meshVertices.Clear();
        meshTriangles.Clear();
        meshUv.Clear();
        meshFilter.mesh.Clear();
        vertexIndex = 0;
        meshFilter.mesh.RecalculateNormals();
    }
    private void AddBlockTextureUV(int atlasesCode)
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
        meshUv.Add(new Vector2(uvX, uvY)); 
        meshUv.Add(new Vector2(uvX, uvY + nh));
        meshUv.Add(new Vector2(uvX + nw, uvY));
        meshUv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private void AddItemTextureUV(int atlasesCode)
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
        meshUv.Add(new Vector2(uvX, uvY));
        meshUv.Add(new Vector2(uvX, uvY + nh));
        meshUv.Add(new Vector2(uvX + nw, uvY));
        meshUv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private IEnumerator CanIsItemGetDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canIsItemGet = true;
    }
    private IEnumerator ActiveFalse()
    {
        yield return new WaitForSeconds(30.0f);
        gameObject.SetActive(false);
    }
}
 