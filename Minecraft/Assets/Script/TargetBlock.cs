using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{
    private readonly int spriteWidth = 10;
    private readonly int spriteHeight = 1;

    private int vertexIndex = 0;
    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();

    private Vector3Int targetBlock;
    private float targetBlockHardness;
    private float stayeMouseButtonTime = 0f;
    private int attack = 1;

    private MeshFilter meshFilter;
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {   // ������ ����
        // Front
        new Vector3(-0.0001f, -0.0001f, -0.0001f), // LB
        new Vector3( 1.0001f, -0.0001f, -0.0001f), // RB
        new Vector3( 1.0001f,  1.0001f, -0.0001f), // RT
        new Vector3(-0.0001f,  1.0001f, -0.0001f), // LT

        // Back
        new Vector3(-0.0001f, -0.0001f, 1.0001f), // LB
        new Vector3( 1.0001f, -0.0001f, 1.0001f), // RB
        new Vector3( 1.0001f,  1.0001f, 1.0001f), // RT
        new Vector3(-0.0001f,  1.0001f, 1.0001f), // LT
    };

    private int bufferSpriteNum = -1;

    void Start()
    {
        MeshInit();
        SetSpriteNum();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            SetTargetInfo();
            stayeMouseButtonTime += Time.deltaTime;
            float percent = stayeMouseButtonTime / targetBlockHardness * (spriteWidth-1);
            SetSpriteNum((int)percent + 1);
            
            if (targetBlockHardness <= stayeMouseButtonTime)
            {
                stayeMouseButtonTime = 0;
                Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(transform.position);
                Vector3Int voxelPos = Utile.Vector3ToVector3Int(result.voxelPos);

                int itemCode = World.Instance.GetChunkFromCoord(result.chunkCoord).chunkMapData.GetVoxelState(result.voxelPos).id;
                World.Instance.GetChunkFromPos(transform.position).
                    ModifyChunkData(voxelPos, CodeData.BLOCK_Air);

                Vector3 vec = new Vector3(+0.5f, +0.5f, +0.5f);
                GameManager.Instance.itemManager.AddDropItem(itemCode, 1, transform.position + vec);
            }
        }
        else
        {
            SetSpriteNum();
            stayeMouseButtonTime = 0;
        }
    }
    private void MeshInit()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();

        for (int face = 0; face < 6; ++face)
        {

            for (int i = 0; i < 4; ++i)
                meshVertices.Add(voxelVerts[VoxelData.voxelTris[face, i]]);


            foreach (int i in ChunkHelperData.vertexData)
                meshTriangles.Add(vertexIndex + i);

            vertexIndex += 4;
        }
        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
    }
    private void AddTextureUV(int atlasesCode)
    {
        // ��Ʋ�� ���� �ؽ��� ����, ���� ����
        int x = atlasesCode;
        int y = 0;
        float nw = (1.0f / spriteWidth);
        float nh = (1.0f / spriteHeight);

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        meshUv.Add(new Vector2(uvX, uvY));
        meshUv.Add(new Vector2(uvX, uvY + nh));
        meshUv.Add(new Vector2(uvX + nw, uvY));
        meshUv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private void SetTargetInfo()
    {
        Vector3Int newBlock = Utile.Vector3ToVector3Int(transform.position);
        if (targetBlock != newBlock)
        {
            stayeMouseButtonTime = 0;

            targetBlock = newBlock;
            Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(targetBlock);
            targetBlockHardness = World.Instance.GetChunkFromCoord(result.chunkCoord).chunkMapData.GetVoxelState(result.voxelPos).properties.hardness;

            if (true)
            {   // ä�� ���� ����
                targetBlockHardness *= 1.5f;
                targetBlockHardness *= attack;
            }
            //else
            //{
            //    targetBlockHardness *= 5.0f;
            //}
        }
        // ä���� �����ϴٸ� (�浵 �� * 1.5)��
        // ä���� �����ϴٸ� (�浵 �� * 5.0)��
        // �⺻ 1x  ���� 2x  ��4x  ö6x  ���̾Ƹ��8x
    }
    private void SetSpriteNum(int num = 0)
    {
        if (bufferSpriteNum == num)
            return;

        bufferSpriteNum = num;
        meshUv.Clear();
        for (int face = 0; face < 6; ++face)
            AddTextureUV(num);
        meshFilter.mesh.uv = meshUv.ToArray();
        meshFilter.mesh.RecalculateNormals();
    }
}
