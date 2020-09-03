using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using TMPro;

public class LowPolyTerrain : MonoBehaviour
{
    protected Vector3[] vertices;
    protected Vector3[,] rawVertices;
    
    protected bool[,] isTerrainHigh;
    protected Color[,] colorInfo;
    protected Vector3[,] normalOfFaces;

    public float xLength, yLength, peakHeight;
    public int xResolution, yResolution;
    private float xTileLength, yTileLength;
    public float[,] gradient;

    public Material highMat, lowMat;

    public TextMeshProUGUI stats;

    public float persistance, lacunarity, scale;



    // Start is called before the first frame update
    public void Start()
    {
        xTileLength = xLength / (xResolution - 1);
        yTileLength = yLength / (yResolution - 1);

        rawVertices = new Vector3[xResolution, yResolution];

        vertices = new Vector3[(xResolution - 1)*(yResolution - 1)*6];
        
        isTerrainHigh = new bool[(xResolution - 1), (yResolution - 1)];

        colorInfo = new Color[(xResolution - 1), (yResolution - 1)];

        normalOfFaces = new Vector3[(xResolution - 1), (yResolution - 1)];

        gradient = new float[xResolution, yResolution];

        fillRawVertices();
        OffsetVertices();
        MakeNoiseMask();
        AddHeights();
        CreateShape();
               
    }


    void fillRawVertices(){
        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){
                rawVertices[x,y] = new Vector3(x * xTileLength, 0f, y * yTileLength);
                //Debug.Log(rawVertices[x,y]);
            }
            //Debug.Log(rawVertices[xResolution - 1,yResolution -1]);
        }
    }

    float offsetPercent = 0.8f;
    void OffsetVertices(){
        float offsetXLimit = offsetPercent * xTileLength/2f;
        float offsetYLimit = offsetPercent * yTileLength/2f;

        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){
                rawVertices[x,y] += new Vector3(Random.Range(-offsetXLimit, offsetXLimit), 0f, Random.Range(-offsetYLimit, offsetYLimit));
            }
        }
        //Debug.Log(rawVertices[xResolution - 1,yResolution -1]);
    }

    void CreateShape(){
        Random.seed = 42;

        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){
                int vert = (y*(xResolution-1) + x) * 6; 

                if((x != xResolution-1) && (y != yResolution-1)){
                    isTerrainHigh[x,y] = false;
                    
                    vertices[vert] = rawVertices[x,y];
                    vertices[vert+1] = rawVertices[x,y+1];
                    vertices[vert+2] = rawVertices[x+1,y];

                    vertices[vert+3] = rawVertices[x,y+1];
                    vertices[vert+4] = rawVertices[x+1,y+1];
                    vertices[vert+5] = rawVertices[x+1,y]; 
                    
                    /*if(x == xResolution - 2 && y == yResolution -2){
                        Debug.Log(vertices[vert]);
                        Debug.Log(vertices[vert+1]);
                        Debug.Log(vertices[vert+2]);
                        Debug.Log(vertices[vert+3]);
                        Debug.Log(vertices[vert+4]);
                        Debug.Log(vertices[vert+5]);
                    }*/
                }
                
            }
        }
    }

    

    void AddHeights(){

        Random.seed = System.DateTime.Now.Millisecond;


        int seed = Random.Range(0, 50);
        
        persistance = Random.Range(0.2f, 0.7f);
        lacunarity = Random.Range(1f, 1.5f);
        scale = (lacunarity < 1.3f) ? Random.Range(7f, 10f): Random.Range(10f, 15f);
        Vector3 offset = new Vector3( Random.Range(-500f, 500f), 0f, Random.Range(-500f, 500f) );

        
        float[,] noise = Noise.GenerateNoiseMap(xResolution, yResolution, seed, scale, 4, persistance, lacunarity, offset, 0);


        peakHeight = 0;
        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){

                float yDist = Mathf.PerlinNoise(rawVertices[x,y].x * 0.3f, rawVertices[x,y].z * 0.3f ) * 1f;
                rawVertices[x,y] += Vector3.up * noise[x,y] * 35f * gradient[x,y]  + Vector3.up*yDist;
                
                if(rawVertices[x,y].y > peakHeight)
                    peakHeight = rawVertices[x,y].y;

            }
        }
        Debug.Log(peakHeight);
        //stats.text = "Scale: " + scale + "\nPersistance:" + persistance + "\nLacunarity: " + lacunarity + "\nPeak Height:" + peakHeight;


        //Debug.Log(rawVertices[xResolution - 1,yResolution -1]);
    }


    void MakeNoiseMask(){
        float gradientRate = 0.1f;
        float percentA = 40f/140f;
        float percentB = 120f/140f;
        float percentD = 6f/50f;

        float distA = 60f; float distB = 150f;
        float distD = 10f;

        int pointA = Mathf.RoundToInt(distA/yTileLength);
        int pointB = Mathf.RoundToInt(distB/yTileLength);
        int pointC = xResolution/2;
        int pointD = Mathf.RoundToInt(distD/xTileLength);
       

        for(int y = 0; y < yResolution; y ++){
            for(int x = 0; x < xResolution; x++){
                gradient[x,y] = 0.1f;
                if(x >= pointC){
                    gradient[x,y] = Mathf.Clamp01(gradientRate * ((x-pointC)-pointD+1));
                }
                if(x < pointC){
                    gradient[x,y] = Mathf.Clamp01(-gradientRate * ((x-pointC)+pointD));
                }
                if(y <= pointA){
                    gradient[x,y] += Mathf.Clamp01( (gradientRate) * ((pointA-y)));
                }
                if(y > pointB){
                    gradient[x,y] += Mathf.Clamp01(-gradientRate * (pointB-y));
                }
                gradient[x,y] = Mathf.Clamp(gradient[x,y], 0f, 1f);
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
