using UnityEngine;
public class Perlin
{
    static public int GetPerlinNoise(in ChunkCoord coord, in int seed, int x, int z)
    {
        x += (coord.x * 16);
        z += (coord.z * 16);
        int scale = 30;
        int minHeight = 20;
        float height = Mathf.PerlinNoise((float)((float)x / scale) + seed, (float)((float)z / scale) + seed);
        int heightValue = Mathf.RoundToInt(height * 10);
        return heightValue + minHeight;
    }

    //{
    //    density = (noise + cliff) * scale + offset - y
    //    cliff = (noise * 0.5 + 0.5) * cliffScale
    //    사용한 노이즈 함수 : Simplex noise
    //    noise : 노이즈 함수 값.Noise(x, y, z)
    //    cliffScale : 지형 굴곡
    //    scale : 수치를 키우기 위한 임의의 값
    //    offset : 터레인의 최소높이
    //    y : 현재 y좌표
    //}

    public static float GetPerlinNoiseTerrain(in ChunkCoord coord, in int seed, in Vector3Int pos, in float scale = 15)
    {
        float X = (coord.x * VoxelData.ChunkWidth) + pos.x;
        float Y = pos.y;
        float Z = (coord.z * VoxelData.ChunkWidth) + pos.z;
        X = (X / scale) + seed;
        Y = (Y / scale) + seed;
        Z = (Z / scale) + seed;

        float XY = Mathf.PerlinNoise(X, Y);
        float YZ = Mathf.PerlinNoise(Y, Z);
        float ZX = Mathf.PerlinNoise(Z, X);

        float YX = Mathf.PerlinNoise(Y, X);
        float ZY = Mathf.PerlinNoise(Z, Y);
        float XZ = Mathf.PerlinNoise(X, Z);
        float minHight = 20;
        float density = (XY + YZ + ZX + YX + ZY + XZ) / 6f;
        density = density * 40 + minHight - pos.y;
        return density;
    }
    public static float GetPerlinNoiseCave(in ChunkCoord coord, in int seed, in Vector3Int pos, in float scale = 15)
    {
        float X = (coord.x * VoxelData.ChunkWidth) + pos.x;
        float Y = pos.y;
        float Z = (coord.z * VoxelData.ChunkWidth) + pos.z;
        X = (X / scale) + (seed*3);
        Y = (Y / scale) + (seed*3);
        Z = (Z / scale) + (seed*3);

        float XY = Mathf.PerlinNoise(X, Y);
        float YZ = Mathf.PerlinNoise(Y, Z);
        float ZX = Mathf.PerlinNoise(Z, X);

        float YX = Mathf.PerlinNoise(Y, X);
        float ZY = Mathf.PerlinNoise(Z, Y);
        float XZ = Mathf.PerlinNoise(X, Z);
        float value = (XY + YZ + ZX + YX + ZY + XZ) / 6f;
        value *= 1.5f;
        //val -= (y / 50);
        return value;
    }

}

