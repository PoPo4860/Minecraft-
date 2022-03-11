using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <���ڵ�, �ؽ��� ����>�� ����. </summary>
    private static readonly Dictionary<ushort, ushort[]> BlockInfo = new Dictionary<ushort, ushort[]>();
    
    // ���� �����ڴ� �ν��Ͻ��� ó�� ȣ��ɋ� �����ڰ� ȣ��ȴ�.
    static CodeData()
    {
        ReadToBlockInfo();
    }

    public static ushort GetBlockTextureAtlases(ushort blockCode, int num)
    {
        ushort[] textureAtlases = new ushort[6];
        if (false == BlockInfo.TryGetValue(blockCode, out textureAtlases))
        {
            UnityEngine.Debug.LogError("�� �ڵ尪 �߸���µ�? �����ž�");
        };
        return textureAtlases[num];
    }
    public static int GetBolckInfoSize()
    {
        return BlockInfo.Count;
    }
    private static void ReadToBlockInfo()
    {
        // �о�� text file �� ��θ� ���� �մϴ�.
        // text file �� ������ ���� �� �о�� string �迭�� ���� �մϴ�.
        string[] textValue = File.ReadAllLines(@"Assets\CodeInfo\TEXT_File.txt");
        for (int i = 0; i < textValue.Length; ++i)
        {
            string[] words = textValue[i].Split(',');

            ushort blockCode;
            UInt16.TryParse(words[0], out blockCode);
            
            ushort[] TextureAtlasesNum = new ushort[6];
            for (int j = 1; j < 7; ++j) 
            {
                UInt16.TryParse(words[j], out TextureAtlasesNum[j - 1]);
            }
            BlockInfo.Add(blockCode, TextureAtlasesNum);
        }
    }
}


