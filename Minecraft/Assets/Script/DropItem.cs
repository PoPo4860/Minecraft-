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

    public bool canIsItemGet = false;

    void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = World.Instance.TextureAtlas;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0));
    }

    private void OnEnable()
    {
        SetItemRender(itemCode);
        StartCoroutine(CanIsItemGetDelay());
    }

    private void OnDisable()
    {
        ClearItemRender();
    }
    private void SetItemRender(int itemCode)
    {
        Vector3 vec = new Vector3(-0.5f, +0.5f, -0.5f);
        for (int face = 0; face < 6; ++face)
        {
            for (int i = 0; i < 4; ++i)
                meshVertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[face, i]] + vec);

            foreach (int i in ChunkHelperData.vertexData)
                meshTriangles.Add(vertexIndex + i);
            int atlasesCode = CodeData.GetBlockTextureAtlases(itemCode, face);

            AddTextureUV(atlasesCode);
            vertexIndex += 4;
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
        vertexIndex = 0;
        meshFilter.mesh.RecalculateNormals();
    }
    IEnumerator CanIsItemGetDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canIsItemGet = true;
    }

    private void AddTextureUV(int atlasesCode)
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


}
