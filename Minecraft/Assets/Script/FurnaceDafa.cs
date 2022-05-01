using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FurnaceDafa
{
    public static float GetCombustionValue(int code) => code switch
    {
        CodeData.Item_Stick => 5f,
        CodeData.BLOCK_OkaPlanks => 15f,
        CodeData.BLOCK_OakTree => 15f,
        CodeData.BLOCK_CraftingTable => 15f,
        CodeData.Item_Coal => 80f,
        _ => 0,
    };
    public static float GetBakeResultCode(int code) => code switch
    {
        CodeData.BLOCK_Coal => CodeData.Item_Coal,
        CodeData.BLOCK_OakTree => CodeData.Item_CharCoal,    // 목탄 예정
        CodeData.BLOCK_Iron => CodeData.Item_IronIngot,     // 철 원석 예정
        CodeData.BLOCK_Diamond => CodeData.Item_Diamond,     // 다이아몬드 원석 예정
        CodeData.BLOCK_CobbleStones => CodeData.BLOCK_Stone,
        _ => 0,
    };
}
