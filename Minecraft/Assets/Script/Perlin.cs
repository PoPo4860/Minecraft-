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
    public static float GetNoiseAt(int x, int z, int seed ,float scale, float heightMultiplier, int octaves, float persistance, float lacunarity)
    {
        float PerlinValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            // Get the perlin value at that octave and add it to the sum
            PerlinValue += Mathf.PerlinNoise(x * frequency + seed, z * frequency + seed) * amplitude;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value
        return PerlinValue * heightMultiplier;
    }
    /// scale : [ perlin noise ]뷰의 스케일
    /// highMultiplier : 지형의 최대 높이
    /// octaves : 반복 횟수 (많을수록 지형이 상세해집니다)
    /// persion : 높을수록 지형이 거칠어집니다(이 값은 0에서 1 사이여야 합니다).
    /// lacunity : 높을수록 지형이 더 많은 "특징"을 갖게 됩니다(양수여야 하낟).
    /// 

    public static float GetPerlinNoiseTerrain(in ChunkCoord coord, in int seed, in int x, in int y, in int z, in float scale = 15)
    {
        float X = (coord.x * VoxelData.ChunkWidth) + x;
        float Y = y;
        float Z = (coord.z * VoxelData.ChunkWidth) + z;
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
        float value = (XY + YZ + ZX + YX + ZY + XZ) / 6f;
        value = value * 40 + minHight - y;
        
        return value;
    }
    public static float GetPerlinNoiseCave(in ChunkCoord coord, in int seed, in int x, in int y, in int z, in float scale = 15)
    {
        float X = (coord.x * VoxelData.ChunkWidth) + x;
        float Y = y;
        float Z = (coord.z * VoxelData.ChunkWidth) + z;
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
