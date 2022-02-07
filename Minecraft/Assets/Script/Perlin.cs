using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Perlin : MonoBehaviour
{
    public int size;        // �� ������
    public GameObject cube; // ������ ������Ʈ
    public float scale;     // ���� �������� ������ ���� ŭ.
    public float m;         // �ִ� ������.
    public float height = 0;
    public int newNoise;
    void Start()
    {
        // �� ����
        newNoise = Random.Range(0, 10000);
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                GameObject child = Instantiate(cube, new Vector3(x, 0, z), Quaternion.identity);

                child.transform.parent = transform;
            }
        }

        // �� ��ġ ����
        foreach (Transform child in transform)
        {
            height = GetPerlinNoise(child);
            child.transform.position = new Vector3(child.transform.position.x, Mathf.RoundToInt(height * m), child.transform.position.z);

            // Mathf.RoundToInt() : �Ű������� ���� �Ҽ��� ����� ������ ��ȯ�Ѵ�.
        }

        // �� �� ����
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
