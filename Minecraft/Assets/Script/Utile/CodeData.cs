using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <���ڵ�, �ؽ��� ����>�� ����. </summary>
    private static readonly Dictionary<ushort, BlockType> BlockInfo = new Dictionary<ushort, BlockType>();
    static CodeData()
    {
        ReadToBlockInfo();
    }
    public static ushort GetBlockTextureAtlases(ushort blockCode, int num)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType textureAtlases))
        {
            UnityEngine.Debug.LogError("�� �ڵ尪 �߸���µ�? �����ž�");
        };
        return textureAtlases.textureAtlases[num];
    }
    public static BlockType GetBlockInfo(ushort blockCode)
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
            ushort.TryParse(words[1], out ushort blockCode);
            for (int j = 2; j <= 7; ++j)
                UInt16.TryParse(words[j], out newBlock.textureAtlases[j - 2]);
            bool.TryParse(words[8], out newBlock.isSolid);
            bool.TryParse(words[9], out newBlock.renderNeighborFaces);
            byte.TryParse(words[10], out newBlock.opacity);
            BlockInfo.Add(blockCode, newBlock);
        }
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


