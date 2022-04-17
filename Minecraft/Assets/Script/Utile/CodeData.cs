using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <���ڵ�, �ؽ��� ����>�� ����. </summary>
    private static readonly Dictionary<int, BlockType> BlockInfo = new Dictionary<int, BlockType>();
    static CodeData()
    {
        ReadToBlockInfo();
    }
    public static ushort GetBlockTextureAtlases(int blockCode, int num)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType textureAtlases))
        {
            UnityEngine.Debug.LogError("�� �ڵ尪 �߸���µ�? �����ž�");
        };
        return textureAtlases.textureAtlases[num];
    }
    public static BlockType GetBlockInfo(int blockCode)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType searchBlockType))
        {
            UnityEngine.Debug.LogError("�� �ڵ尪 �߸���µ�? �����ž�");
        };
        return searchBlockType;
    }

    private static void ReadToBlockInfo()
    {
        // �о�� text file �� ��θ� ���� �մϴ�.
        // text file �� ������ ���� �� �о�� string �迭�� ���� �մϴ�.
        string[] textValue = File.ReadAllLines(@"Assets\CodeInfo\TEXT_File.txt");
        for (int i = 0; i < textValue.Length; ++i)
        {
            string[] words = textValue[i].Split(',');
            BlockType newBlock = new BlockType();

            newBlock.blockName = words[0];
            ushort blockCode = ushort.Parse(words[1]);
            for (int j = 2; j <= 7; ++j)
                newBlock.textureAtlases[j - 2] = UInt16.Parse(words[j]);
            newBlock.isSolid = bool.Parse(words[8]);
            newBlock.renderNeighborFaces = bool.Parse(words[9]);
            newBlock.hardness = float.Parse(words[10]);
            newBlock.type = SetBlockType(words[11]);

            BlockInfo.Add(blockCode, newBlock);
            
        }
    }
    private static Type SetBlockType(string str)
    {
        return str switch
        {
            "Basic" => Type.Basic,
            "Soil" => Type.Soil,
            "Stone" => Type.Stone,
            "Wood" => Type.Wood,
            _ => Type.Basic,
        };
    }
    public static readonly ushort BLOCK_AIR = 0;
    public static readonly ushort BLOCK_STONE = 1;
    public static readonly ushort BLOCK_GRASS = 2;
    public static readonly ushort BLOCK_DIRT = 3;
    public static readonly ushort BLOCK_BEDROCK = 7;
    public static readonly ushort BLOCK_IRON = 15;
    public static readonly ushort BLOCK_COAL = 16;
    public static readonly ushort BLOCK_DIAMOND = 56;
    public static readonly ushort BLOCK_OAKTREE = 17;
    public static readonly ushort BLOCK_LEAF = 18;
}


