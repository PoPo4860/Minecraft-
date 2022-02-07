using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    public static readonly int TextureAtlasWidth = 20;
    public static readonly int TextureAtlasHeight = 20;

    public static readonly int WorldSizeInChunks = 10;

    // 텍스쳐 아틀라스 내에서 각 행, 열마다 텍스쳐가 갖는 크기 비율
    public static readonly float NormalizedTextureAtlasWidth = 1f / TextureAtlasWidth;
    public static readonly float NormalizedTextureAtlasHeight = 1f / TextureAtlasHeight;

    public static readonly int ChunkWidth = 10;
    public static readonly int ChunkHeight = 10;
    /// <summary> 큐브의 8개 버텍스의 상대 위치 </summary>
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {   // 정점을 설정
        // Front
        new Vector3(0.0f, 0.0f, 0.0f), // LB
        new Vector3(1.0f, 0.0f, 0.0f), // RB
        new Vector3(1.0f, 1.0f, 0.0f), // RT
        new Vector3(0.0f, 1.0f, 0.0f), // LT

        // Back
        new Vector3(0.0f, 0.0f, 1.0f), // LB
        new Vector3(1.0f, 0.0f, 1.0f), // RB
        new Vector3(1.0f, 1.0f, 1.0f), // RT
        new Vector3(0.0f, 1.0f, 1.0f), // LT
    };


    /// <summary> 큐브의 각 면을 이루는 삼각형들의 버텍스 인덱스 데이터 </summary>
    public static readonly int[,] voxelTris = new int[6, 4]
    {   // 정점을 이어 준다.
    {0, 3, 1, 2 }, // Back Face   (-Z)
    {5, 6, 4, 7 }, // Front Face  (+Z)
    {3, 7, 2, 6 }, // Top Face    (+Y)
    {1, 5, 0, 4 }, // Bottom Face (-Y)
    {4, 7, 0, 3 }, // Left Face   (-X)
    {1, 2, 5, 6 }, // RIght Face  (+X)
    };

    /// <summary> voxelTris의 버텍스 인덱스 순서에 따라 정의된 UV 좌표 데이터 </summary>
    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {   // uvs 빛이 반사되는 방향
    new Vector2(0.0f, 0.0f), // LB
    new Vector2(0.0f, 1.0f), // LT
    new Vector2(1.0f, 0.0f), // RB
    new Vector2(1.0f, 1.0f), // RT
    };

    /// <summary> 정6면체 각각의 앞의 좌표값 </summary>
    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
    new Vector3( 0.0f,  0.0f, -1.0f), // Back Face   (-Z)
    new Vector3( 0.0f,  0.0f, +1.0f), // Front Face  (+Z)
    new Vector3( 0.0f, +1.0f,  0.0f), // Top Face    (+Y)
    new Vector3( 0.0f, -1.0f,  0.0f), // Bottom Face (-Y)
    new Vector3(-1.0f,  0.0f,  0.0f), // Left Face   (-X)
    new Vector3(+1.0f,  0.0f,  0.0f), // RIght Face  (+X)
    };
}
