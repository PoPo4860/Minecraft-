using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRightHand : MonoBehaviour
{
    #region
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private int vertexIndex = 0;
    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();
    #endregion

    public Transform handTransform;
    private bool handDownCheck = false;
    private bool handUpCheck = false;
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = World.Instance.TextureAtlasTrans;
    }

    void Update()
    {
        if(true == handDownCheck)
        {
            if(handTransform.localRotation.x < 0.2f)
            {
                handTransform.Rotate(new Vector3(300f * Time.deltaTime, 0, 0));
            }
            else
            {
                handDownCheck = false;
                handUpCheck = true;
            }
        }
        if(true == handUpCheck)
        {
            if (0f < handTransform.localRotation.x)
            {
                handTransform.Rotate(new Vector3(-300f * Time.deltaTime, 0, 0));
            }
            else
            {
                handUpCheck = false;
                handTransform.localRotation = new Quaternion();
            }
        }
    }
    public void ActiveHandMove()
    {
        if (false == handDownCheck &&
            false == handUpCheck)
        {
            handDownCheck = true;
        }
    }
    public void SetItemRender(int itemCode)
    {
        ClearItemRender();
        if (0 == itemCode)
            return;
        
        Vector3 vec = new Vector3(-0.5f, +0.5f, -0.5f);
        if (null != CodeData.GetBlockInfo(itemCode))
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
        vertexIndex = 0;
        meshFilter.mesh.Clear();
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
}