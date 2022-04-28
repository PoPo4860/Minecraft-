using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <블럭코드, 텍스쳐 정보>가 담긴다. </summary>
    private static readonly Dictionary<int, BlockType> BlockInfo = new Dictionary<int, BlockType>();
    private static readonly Dictionary<int, ItemType> ItemInfo = new Dictionary<int, ItemType>();
    static CodeData()
    {
        ReadToBlockInfo();
        ReadToItemInfo();
    }
    public static ushort GetBlockTextureAtlases(int blockCode, int num)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType textureAtlases))
        {
            UnityEngine.Debug.LogError("블럭 코드값 잘못줌");
        };
        return textureAtlases.textureAtlases[num];
    }
    public static BlockType GetBlockInfo(int blockCode)
    {
        if (false == BlockInfo.TryGetValue(blockCode, out BlockType searchBlockType))
        {
            searchBlockType = null;
        };
        return searchBlockType;
    }

    public static ushort GetItemTextureAtlases(int itemCode)
    {
        if (false == ItemInfo.TryGetValue(itemCode, out ItemType textureAtlases))
        {
            UnityEngine.Debug.LogError("아이템 코드값 잘못줌");
        };
        return textureAtlases.textureAtlases;
    }
    public static ItemType GetItemInfo(int itemCode)
    {
        if (false == ItemInfo.TryGetValue(itemCode, out ItemType searchBlockType))
        {
            searchBlockType = null;
        };
        return searchBlockType;
    }

    public static string GetCodeName(int itemCode)
    {
        string str = "";
        if(null != GetBlockInfo(itemCode))
            str = GetBlockInfo(itemCode).blockName;
        else
            str = GetItemInfo(itemCode).itemName;
        return str;
    }

    private static void ReadToBlockInfo()
    {
        // 읽어올 text file 의 경로를 지정 합니다.
        // text file 의 내용을 한줄 씩 읽어와 string 배열에 대입 합니다.
        string[] textValue = File.ReadAllLines(@"Assets\CodeInfo\BlockInfo.txt");
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
    private static EBlockType SetBlockType(string str)
    {
        return str switch
        {
            "Basic" => EBlockType.Basic,
            "Soil" => EBlockType.Soil,
            "Stone" => EBlockType.Stone,
            "Wood" => EBlockType.Wood,
            _ => EBlockType.Basic,
        };
    }
    private static void ReadToItemInfo()
    {
        string[] textValue = File.ReadAllLines(@"Assets\CodeInfo\ItemInfo.txt");
        for (int i = 0; i < textValue.Length; ++i)
        {
            string[] words = textValue[i].Split(',');
            ItemType newItem = new ItemType();
            newItem.itemName = words[0];
            ushort blockCode = ushort.Parse(words[1]);
            newItem.textureAtlases = UInt16.Parse(words[2]);
            newItem.value = UInt16.Parse(words[2]);
            newItem.type = SetItemType(words[3]);

            ItemInfo.Add(blockCode, newItem);

        }
    }
    private static EItemType SetItemType(string str)
    {
        return str switch
        {
            "Basic" => EItemType.Basic,
            "Sword" => EItemType.Sword,
            "Axe" => EItemType.Axe,
            "Pick" => EItemType.Pick,
            "Shovel" => EItemType.Shovel,
            _ => EItemType.Basic,
        };
    }

    public static readonly ushort BLOCK_Air = 0;
    public static readonly ushort BLOCK_Stone = 1;
    public static readonly ushort BLOCK_Grass = 2;
    public static readonly ushort BLOCK_Dirt = 3;
    public static readonly ushort BLOCK_CobbleStones = 4;
    public static readonly ushort BLOCK_OkaPlanks = 5;
    public static readonly ushort BLOCK_BedRock = 7;
    public static readonly ushort BLOCK_Iron = 15;
    public static readonly ushort BLOCK_Coal = 16;
    public static readonly ushort BLOCK_OakTree = 17;
    public static readonly ushort BLOCK_Leaf = 18;
    public static readonly ushort BLOCK_Diamond = 56;
    public static readonly ushort BLOCK_CraftingTable = 58;
    public static readonly ushort Item_Coal = 263;
    public static readonly ushort Item_Stick = 280;

    public enum ECodeType { Empty, Block, Item };

    public static ECodeType GetCodeType(int code)
    {
        //if (BLOCK_AIR == code)
        //    return ECodeType.Empty;

        if (BLOCK_Air == code ||
            BLOCK_Stone == code ||
            BLOCK_Grass == code ||
            BLOCK_Dirt == code ||
            BLOCK_CobbleStones == code ||
            BLOCK_OkaPlanks == code ||
            BLOCK_BedRock == code ||
            BLOCK_Iron == code ||
            BLOCK_Coal == code ||
            BLOCK_OakTree == code ||
            BLOCK_Leaf == code ||
            BLOCK_Diamond == code ||
            BLOCK_CraftingTable == code)
            return ECodeType.Block;

        if (BLOCK_Air == code ||
            Item_Coal == code ||
            Item_Stick == code)
            return ECodeType.Item;

        return ECodeType.Empty;
    }
}


