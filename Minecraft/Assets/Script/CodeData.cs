using System;
using System.Collections.Generic;
using System.IO;
public static class CodeData
{
    /// <summary> <블럭코드, 텍스쳐 정보>가 담긴다. </summary>
    private static readonly Dictionary<ushort, ushort[]> BlockInfo = new Dictionary<ushort, ushort[]>();
    
    // 정적 생성자는 인스턴스가 처음 호출될떄 생성자가 호출된다.
    static CodeData()
    {
        ReadToBlockInfo();
    }

    public static ushort GetBlockTextureAtlases(ushort blockCode, int num)
    {
        ushort[] textureAtlases = new ushort[6];
        if (false == BlockInfo.TryGetValue(blockCode, out textureAtlases))
        {
            UnityEngine.Debug.LogError("블럭 코드값 잘못줬는데? 병형신아");
        };
        return textureAtlases[num];
    }
    public static int GetBolckInfoSize()
    {
        return BlockInfo.Count;
    }
    private static void ReadToBlockInfo()
    {
        // 읽어올 text file 의 경로를 지정 합니다.
        // text file 의 내용을 한줄 씩 읽어와 string 배열에 대입 합니다.
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


