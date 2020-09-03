using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LowPolyMesh : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        


        CreateShape();
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    void CreateShape(){
        vertices = new Vector3[] {
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 1f),
            new Vector3 (1f, 0f, 0f),

            new Vector3 (0f, 0f, 1f),
            new Vector3 (1f, 0f, 1f),
            new Vector3 (1f, 0f, 0f),


        };

        triangles = new int[]{
            0, 1, 2,
            3, 4, 5 
        };

        colors = new Color[]{
            Color.green, Color.green, Color.green,
            Color.green//, Color.red, Color.red
        };
    }

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
