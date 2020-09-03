using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPolyHighMat : LowPolyTerrain
{

    Mesh mesh;

    
    int[] triangles;
    Color[] colors;



    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        triangles = new int[(xResolution - 1)*(yResolution - 1)*6];
        NewUpdateTriangles();
        UpdateMesh();
    }

    void NewUpdateTriangles(){
        int tris = 0;
        for(int y = 0; y < yResolution-1; y++){
            for(int x = 0; x < xResolution-1; x++){
                int vert = (y*(xResolution-1) + x) * 6; 
                
                if(isTerrainHigh[x,y] == true){
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + 1;
                    triangles[tris + 2] = vert + 2;
                    triangles[tris + 3] = vert + 3;
                    triangles[tris + 4] = vert + 4;
                    triangles[tris + 5] = vert + 5;

                    tris += 6;
                }

            }
        }
    }

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    
    void OnDrawGizmos(){
        if(vertices == null)
            return;
        for(int i = 0; i < vertices.Length ; i++ ){
            Gizmos.DrawSphere(vertices[i], .1f);  
        }        
    }
    


}
