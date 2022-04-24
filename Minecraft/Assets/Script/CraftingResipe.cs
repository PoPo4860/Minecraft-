using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CraftingResipe
{
    private static List<CraftingResipeNode> list = new List<CraftingResipeNode>();
    private static List<CraftingResipeNode> recipeList;
    static CraftingResipe()
    {
        int[] recipe = new int[10]
            {1,2,3,4,5,6,7,8,9,10};
        SetResipe(recipe);
    }
    public static void SetResipe(int[] recipe)
    {
        List<CraftingResipeNode> baseList = list;
        recipeList = list;
        for (int i = 0; i < 10; ++i)
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

    public static int GetResipe(int[] recipe)
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
                return 0;
        }
        return recipeList[0].itemCode;
    }
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
