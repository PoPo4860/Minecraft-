using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Perlin : MonoBehaviour
{
    public int size;        // 맵 사이즈
    public GameObject cube; // 생성할 오브젝트
    public float scale;     // 값이 적을수록 높낮이 폭이 큼.
    public float m;         // 최대 높낮이.
    public float height = 0;
    public int newNoise;
    void Start()
    {
        // 블럭 생성
        newNoise = Random.Range(0, 10000);
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                GameObject child = Instantiate(cube, new Vector3(x, 0, z), Quaternion.identity);

                child.transform.parent = transform;
            }
        }

        // 블럭 위치 조정
        foreach (Transform child in transform)
        {
            height = GetPerlinNoise(child);
            child.transform.position = new Vector3(child.transform.position.x, Mathf.RoundToInt(height * m), child.transform.position.z);

            // Mathf.RoundToInt() : 매개변수로 받은 소수를 가까운 정수로 반환한다.
        }

        // 블럭 색 조정
        foreach (Transform child in transform)
        {
            height = GetPerlinNoise(child);

            child.GetComponent<MeshRenderer>().material.color = new Color(height, height, height, height);
        }
    }

    void Update()
    {

    }

    float GetPerlinNoise(Transform child)=> Mathf.PerlinNoise((child.transform.position.x / scale) + newNoise, (child.transform.position.z / scale) + newNoise);
}
