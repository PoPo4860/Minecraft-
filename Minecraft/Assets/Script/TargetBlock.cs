using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{
    private readonly int spriteWidth = 10;
    private readonly int spriteHeight = 1;

    private readonly List<Vector3> meshVertices = new List<Vector3>();
    private readonly List<int> meshTriangles = new List<int>();
    private readonly List<Vector2> meshUv = new List<Vector2>();
    private int vertexIndex = 0;
    private MeshFilter meshFilter;
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {   // 정점을 설정
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

    private Vector3Int targetBlock;
    private float targetBlockHardness;
    
    private float stayeMouseButtonTime = 0f;
    private ItemType itemType;
    private int itemTypeNum = 0;
    private bool miningPossible = false;
    private int bufferSpriteNum = -1;
    void Start()
    {
        MeshInit();
        SetSpriteNum();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            SetTargetInfo();
            stayeMouseButtonTime += Time.deltaTime;
            float percent = stayeMouseButtonTime / targetBlockHardness * (spriteWidth-1);
            SetSpriteNum((int)percent + 1);
            
            if (targetBlockHardness <= stayeMouseButtonTime)
            {   // 블럭이 파괴되었을 때에
                stayeMouseButtonTime = 0;
                Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(transform.position);
                Vector3Int voxelPos = Utile.Vector3ToVector3Int(result.voxelPos);

                int itemCode = Utile.GetVoxelStateFromWorldPos(transform.position).id;
                World.Instance.GetChunkFromPos(transform.position).
                    ModifyChunkData(voxelPos, CodeData.BLOCK_Air);

                if (CodeData.BLOCK_Furnace == itemCode ||
                    CodeData.BLOCK_FurnaceFire == itemCode) 
                {
                    Vector3Int targetPos = Utile.Vector3ToVector3Int(transform.position);
                    GameManager.Instance.uiManager.furncaeUI.DeletFurnace(targetPos);
                }
                Vector3 vec = new Vector3(+0.5f, +0.5f, +0.5f);
                if(true == miningPossible)
                {
                    int dropItemCode = GetDropItemCode(itemCode);
                    GameManager.Instance.itemManager.AddDropItem(dropItemCode, 1, transform.position + vec);
                }
            }
        }
        else
        {
            SetSpriteNum(0);
            stayeMouseButtonTime = 0;
        }
    }
    public void SetItemType(ItemType itemType)
    {
        this.itemType = itemType;
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
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        int x = atlasesCode;
        int y = 0;
        float nw = (1.0f / spriteWidth);
        float nh = (1.0f / spriteHeight);

        float uvX = x * nw;
        float uvY = y * nh;

        // 해당 텍스쳐의 uv를 LB-LT-RB-RT 순서로 추가
        meshUv.Add(new Vector2(uvX, uvY));
        meshUv.Add(new Vector2(uvX, uvY + nh));
        meshUv.Add(new Vector2(uvX + nw, uvY));
        meshUv.Add(new Vector2(uvX + nw, uvY + nh));
    }
    private void SetTargetInfo()
    {
        Vector3Int blockPos = Utile.Vector3ToVector3Int(transform.position);
        int currentItemTypeNum;
        if(null == itemType)
            currentItemTypeNum = 0;
        else
            currentItemTypeNum = itemType.textureAtlases;

        if (targetBlock != blockPos ||
            itemTypeNum != currentItemTypeNum)
        {
            targetBlock = blockPos;
            itemTypeNum = currentItemTypeNum;
            stayeMouseButtonTime = 0;
            Utile.ChunkCoordInPos result = Utile.GetCoordInVoxelPosFromWorldPos(targetBlock);
            Voxel voxel = World.Instance.GetChunkFromCoord(result.chunkCoord).chunkMapData.GetVoxelState(result.voxelPos);
            targetBlockHardness = voxel.blockProperties.hardness;
            if (true == GetMiningCheck(voxel , out float value))
            {   // 채굴 가능 여부
                miningPossible = true;
                targetBlockHardness *= 1.5f;
                targetBlockHardness /= value;
            }
            else
            {
                miningPossible = false;
                targetBlockHardness *= 5.0f;
            }
        }
        // 채굴이 가능하다면 (경도 값 * 1.5)초
        // 채굴이 가능하다면 (경도 값 * 5.0)초
        // 기본 1x  나무 2x  돌4x  철6x  다이아몬드8x
    }
    private bool GetMiningCheck(Voxel voxel, out float value)
    {
        value = 1f;

        if (null == itemType )
        {
            if(EBlockType.Stone == voxel.blockProperties.type)
                return false;
            else
                return true;
        }

        if (EItemType.Axe == itemType.type &&
            EBlockType.Wood == voxel.blockProperties.type)
            value = itemType.value;
        
        if(EBlockType.Stone == voxel.blockProperties.type)
        {
            if (EItemType.Pick == itemType.type)
            {
                value = itemType.value;
                switch (itemType.itemName)
                {
                    case "다이아몬드 곡괭이":
                        goto case "철 곡괭이";

                    case "철 곡괭이":
                        if (CodeData.BLOCK_Diamond == voxel.id)
                            return true;
                        goto case "돌 곡괭이";
                    
                    case "돌 곡괭이":
                        if (CodeData.BLOCK_Coal == voxel.id ||
                            CodeData.BLOCK_Iron == voxel.id)
                            return true;
                        goto case "나무 곡괭이";
                    
                    case "나무 곡괭이":
                        if (CodeData.BLOCK_Stone == voxel.id ||
                            CodeData.BLOCK_CobbleStones == voxel.id ||
                            CodeData.BLOCK_Furnace == voxel.id ||
                            CodeData.BLOCK_FurnaceFire == voxel.id)
                            return true;
                        return false;

                    default:
                        return false;
                }
            }
            else
                return false;
        }

        if (EItemType.Shovel == itemType.type &&
            EBlockType.Soil == voxel.blockProperties.type)
            value = itemType.value;
        
        if (EBlockType.Basic == voxel.blockProperties.type)
            value = itemType.value;
        
        return true;
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
    private int GetDropItemCode(int destoryBlockCode)
    {
        return destoryBlockCode switch
        {
            CodeData.BLOCK_Stone => CodeData.BLOCK_CobbleStones,
            CodeData.BLOCK_Coal => CodeData.Item_Coal,
            CodeData.BLOCK_FurnaceFire => CodeData.BLOCK_Furnace,
            CodeData.BLOCK_Diamond => CodeData.Item_Diamond,
            CodeData.BLOCK_Grass => CodeData.BLOCK_Dirt,
            _ => destoryBlockCode,
        };
    }
}
