using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public VoxelRigidbody rigi;
    public int itemCode;

    #region �޽�������
    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;
    #endregion


    void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = World.Instance.TextureAtlas;
        SetItemRender(itemCode);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0));
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
    private void AddTextureUV(int atlasesCode)
    {
        // ��Ʋ�� ���� �ؽ��� ����, ���� ����
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = atlasesCode % w;
        int y = h - (atlasesCode / w) - 1;

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        meshUv.Add(new Vector2(uvX, uvY));
        meshUv.Add(new Vector2(uvX, uvY + nh));
        meshUv.Add(new Vector2(uvX + nw, uvY));
        meshUv.Add(new Vector2(uvX + nw, uvY + nh));
    }
}
