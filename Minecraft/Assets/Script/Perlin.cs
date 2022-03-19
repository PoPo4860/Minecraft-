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
}
