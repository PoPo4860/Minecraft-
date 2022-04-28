using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CraftingResipe
{
    private static List<CraftingResipeNode> list = new List<CraftingResipeNode>();
    private static List<CraftingResipeNode> recipeList;
    static CraftingResipe()
    {
        int[] recipe = new int[11];
        string[] textValue = File.ReadAllLines(@"Assets\CodeInfo\CraftingResipe.txt");
        for (int i = 0; i < textValue.Length; ++i)
        {
            string[] words = textValue[i].Split(',');
            for(int j = 0; j < 11; ++j)
                recipe[j] = int.Parse(words[j]);

            SetResipe(recipe);
        }
    }
    public static void SetResipe(int[] recipe)
    {
        List<CraftingResipeNode> baseList = list;
        recipeList = list;
        for (int i = 0; i < 11; ++i)
        {
            bool check = false;
            for (int j = 0; j < recipeList.Count; ++j)
            {
                if (recipeList[j].itemCode == recipe[i]) 
                {
                    recipeList = recipeList[j].recipeList;
                    check = true;
                    break;
                }
            }
            if (false == check)
            {
                recipeList.Add(new CraftingResipeNode(recipe[i]));
                recipeList = recipeList[recipeList.Count - 1].recipeList;
            }
        }
        list = baseList;
    }
    
    public static ResipeResult GetResipe(int[] recipe)
    {
        recipeList = list;
        for (int i = 0; i < 9; ++i)
        {
            bool check = false;
            for (int j = 0; j < recipeList.Count; ++j)
            {
                check = true;
                if (recipeList[j].itemCode == recipe[i])
                {
                    recipeList = recipeList[j].recipeList;
                    check = false;
                    break;
                }
            }
            if (true == check)
                return new ResipeResult(0,0);
        }
        return new ResipeResult(recipeList[0].itemCode, recipeList[0].recipeList[0].itemCode);
    }
}

public struct ResipeResult
{
    public ResipeResult(int itemCode, int itemNum)
    {
        this.itemCode = itemCode;
        this.itemNum = itemNum;
    }
    public int itemCode;
    public int itemNum;
}

public class CraftingResipeNode
{
    public List<CraftingResipeNode> recipeList = new List<CraftingResipeNode>();
    public int itemCode = 0;

    public CraftingResipeNode(int itemCode)
    {
        this.itemCode = itemCode;
    }
}
