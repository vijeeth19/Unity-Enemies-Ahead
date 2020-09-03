using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    Color[] colors;
    int[] triangles;

    public int xSize = 2;
    public int zSize = 2;

    public bool[,] meshTileData;
    public LayerMask terrainMask;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshTileData = new bool[xSize, zSize];
        for(int z = 0; z < zSize; z++){
            for(int x = 0; x < xSize; x++){
                meshTileData[x, z] = true;
            }
        }

        CreateShape();
        UpdateMesh();
        

    }


    void CreateShape(){
        vertices = new Vector3[ (xSize + 1) * (zSize + 1) ];
        colors = new Color[vertices.Length];

        //Debug.Log("stop");

        for(int i = 0, z = 11; z < zSize + 1; z++){
            for(int x = -6; x < xSize + 1; x += 2 ){
                float meshY = 0f;
                RaycastHit hit;
                Vector3 rayOrigin = new Vector3(x, 50f, z);
                Ray ray = new Ray(rayOrigin, Vector3.down);

                if(Physics.Raycast(ray, out hit, 100f, terrainMask)){
                    meshY = hit.point.y;
                }

                vertices[i] = new Vector3(x, meshY + 0.2f , z);
                //colors[i] = Color.blue;

                
                //Debug.Log(vertices[i]);
                if(vertices[i] == Vector3.zero){
                    //Debug.Log("BUSTED" + i);
                }

                i++;
            }

        }
        //Debug.Log("start");

        
        


        
        triangles = new int[xSize * zSize * 6];
        
        int vert = 0;
        int tris = 0;
        
        for(int z = 0; z < zSize; z++ ){
            for(int x = 0; x < xSize; x++ ){
                if( meshTileData[x,z] == false ){
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;

                    tris += 6;
                }
                vert++;
                
                
            }
            vert++;
        }
        
        
        
        

    }

    void UpdateMesh(){
        mesh.Clear();
        

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(meshTileData[0,0]);
        CreateShape();
        UpdateMesh();
    }

    void OnDrawGizmos(){

        if(vertices == null)
            return;

        

        for(int i = 0; i < vertices.Length ; i++ ){
            Gizmos.DrawSphere(vertices[i], .1f);
            
        }

        
    }
}
