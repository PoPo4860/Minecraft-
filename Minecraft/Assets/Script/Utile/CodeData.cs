using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <블럭코드, 텍스쳐 정보>가 담긴다. </summary>
    private static readonly Dictionary<int, BlockType> BlockInfo = new Dictionary<int, BlockType>();
    static CodeData()
    {
        ReadToBlockInfo();
    }
    public static ushort GetBlockTextureAtlases(int blockCode, int num)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType textureAtlases))
        {
            UnityEngine.Debug.LogError("블럭 코드값 잘못줬는데? 병형신아");
        };
        return textureAtlases.textureAtlases[num];
    }
    public static BlockType GetBlockInfo(int blockCode)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType searchBlockType))
        {
            UnityEngine.Debug.LogError("블럭 코드값 잘못줬는데? 병형신아");
        };
        return searchBlockType;
    }

    private static void ReadToBlockInfo()
    {
        // 읽어올 text file 의 경로를 지정 합니다.
        // text file 의 내용을 한줄 씩 읽어와 string 배열에 대입 합니다.
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


