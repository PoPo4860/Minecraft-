using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreateTset : MonoBehaviour
{
    public Texture TopTexture;
    public Texture SideTexture;
    public Texture BottomTexture;

    public GameObject   TopMesh;
    public GameObject[] SideMesh = new GameObject[4];
    public GameObject   BottomMesh;

    private Mesh _mesh;
    // Start is called before the first frame update
    void Start()
    {
        // 정점 설정
        Vector3[] vertices = new Vector3[]
        {   new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.5f,   0.5f, -0.5f),
            new Vector3(0.5f,  -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f)};
        int[] triangles = new int[] 
        { 0, 1, 2,
          0, 2, 3};

        // 법선, 빛을 받고 반사하는 방향
        Vector3[] normals = new Vector3[]
        {   new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),};

        // 좌표
        Vector2[] uvs = new Vector2[] { 
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(1f, 0f), new Vector2(0f, 0f)};

        _mesh = new Mesh();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.normals = normals;
        
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();

        SetMesh(TopMesh, TopTexture);
        SetMesh(BottomMesh, BottomTexture);
        foreach (GameObject meshObject in SideMesh)
        {
            SetMesh(meshObject, SideTexture);
        }
    }

    void SetMesh(GameObject meshObject, Texture texture)
    {
        meshObject.GetComponent<MeshFilter>().mesh = _mesh;
        Material material = new Material(Shader.Find("Standard"));
        material.SetTexture("_MainTex", texture);
        meshObject.GetComponent<MeshRenderer>().material = material;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
