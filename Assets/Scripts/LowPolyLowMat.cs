using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPolyLowMat : LowPolyTerrain
{

    Mesh mesh;

    
    int[] triangles;
    Color[] colors;

    public Color[] colorPallete;
    public Color[] upColors, downColors;
    public Color colorOne, colorTwo;


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        

        triangles = new int[(xResolution - 1)*(yResolution - 1)*6];
        colors = new Color[vertices.Length];

        colorPallete = new Color[20];
        InitializeColorPallete();
        NewUpdateTriangles();
        ColorTerrain();
        UpdateMesh();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        
    }

    void InitializeColorPallete(){
        colorPallete[0] = new Color(61f/255f, 183f/255f, 1f, 1f);
        colorPallete[1] = new Color(46f/255f, 209f/255f, 78f/255f, 1f);
        colorPallete[2] = new Color(0f, 1f, 1f, 1f);
        colorPallete[3] = new Color(1f, 1f, 60f/255f, 1f);
        colorPallete[4] = new Color(1f, 168f/255f, 66f/255f, 1f);
        colorPallete[5] = new Color(1f, 67f/255f, 47f/255f, 1f);
        colorPallete[6] = new Color(1f, 73f/255f, 168f/255f, 1f);
        colorPallete[7] = new Color(220f/255f, 52f/255f, 1f, 1f);
        colorPallete[8] = new Color(153f/255f, 71f/255f, 1f, 1f);
        colorPallete[9] = new Color(124f/255f, 121f/255f, 1f, 1f);
        colorPallete[10] = new Color(156f/255f, 1f, 0f, 1f);
        colorPallete[11] = new Color(1f, 0f, 163f/255f, 1f);

    }


    void NewUpdateTriangles(){
        int tris = 0;
        for(int y = 0; y < yResolution-1; y++){
            for(int x = 0; x < xResolution-1; x++){
                int vert = (y*(xResolution-1) + x) * 6; 
                
                if(isTerrainHigh[x,y] == false){
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

    void ColorTerrain(){
        Random.seed = System.DateTime.Now.Millisecond;
        colorOne = downColors[Random.Range(0, downColors.Length)];
        colorTwo = /*Color.white;*/ upColors[Random.Range(0, upColors.Length)];
        if(colorOne == colorTwo){
            colorTwo = Color.white;
        }
        //new Color(0f, 1f, 23f/255f, 1f); 
        
        for(int y = 0; y < yResolution - 1; y++){
            for(int x = 0; x < xResolution - 1; x++){
                int vert = (y*(xResolution-1) + x) * 6; 

                float heightAvg = (vertices[vert] + vertices[vert+1] + vertices[vert+2] + vertices[vert+3] + vertices[vert+4] + vertices[vert+5]).y/6f;
                colors[vert] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));
                colors[vert+1] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));
                colors[vert+2] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));
                colors[vert+3] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));
                colors[vert+4] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));
                colors[vert+5] = Color.Lerp(colorOne, colorTwo, Mathf.InverseLerp(0f, peakHeight-8.5f, heightAvg));

                /*
                if(heightAvg >= 0f && heightAvg <= 3f){
                    colors[vert] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                    colors[vert+1] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                    colors[vert+2] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                    colors[vert+3] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                    colors[vert+4] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                    colors[vert+5] = Color.Lerp(Color.white, colorOne, Mathf.InverseLerp(0f, 3f, heightAvg));
                }
                */

            }
        }
    }

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        Debug.Log(mesh.triangles.Length);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
