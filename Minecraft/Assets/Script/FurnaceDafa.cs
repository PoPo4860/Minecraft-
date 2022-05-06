using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FurnaceDafa
{
    public static readonly float combustionMaxTime = 10;

    public static float GetCombustionValue(int code) => code switch
    {
        CodeData.Item_Stick => 5f,
        CodeData.Item_WoodPickaxe => 10f,
        CodeData.Item_WoodShovel => 10f,
        CodeData.BLOCK_OkaPlanks => 15f,
        CodeData.BLOCK_OakTree => 15f,
        CodeData.BLOCK_CraftingTable => 15f,
        CodeData.Item_Coal => 80f,
        CodeData.Item_CharCoal => 80f,
        _ => 0,
    };
    public static int GetBakeResultCode(int code) => code switch
    {
        CodeData.BLOCK_Coal => CodeData.Item_Coal,
        CodeData.BLOCK_OakTree => CodeData.Item_CharCoal,   
        CodeData.BLOCK_Iron => CodeData.Item_IronIngot,     
        CodeData.BLOCK_Diamond => CodeData.Item_Diamond,    
        CodeData.BLOCK_CobbleStones => CodeData.BLOCK_Stone,
        _ => 0,
    };
}

