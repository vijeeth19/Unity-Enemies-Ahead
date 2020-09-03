using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLowPolyTerrain : MonoBehaviour
{
    protected Vector3[] vertices;
    protected Vector3[,] rawVertices;
    
    protected bool[,] isTerrainHigh;
    protected Color[,] colorInfo;
    protected Vector3[,] normalOfFaces;

    public float xLength, yLength, peakHeight, minHeight;
    public int xResolution, yResolution;
    private float xTileLength, yTileLength;
    public float[,] gradient;

    public Material highMat, lowMat;
    float[,] noise; 

    int terrainSeed;
    float scale;
    float persistance;
    float lacunarity;
    protected Vector3 offset;

    float coveredDist = 0;

    int noiseIndex = 0;

    float terrainMovementSpeed = 10;
    


    // Start is called before the first frame update
    public void Start()
    {

        Random.seed = System.DateTime.Now.Millisecond;


        xTileLength = xLength / (xResolution - 1);
        yTileLength = yLength / (yResolution - 1);



        rawVertices = new Vector3[xResolution, yResolution];

        vertices = new Vector3[(xResolution - 1)*(yResolution - 1)*6];
        
        isTerrainHigh = new bool[(xResolution - 1), (yResolution - 1)];

        colorInfo = new Color[(xResolution - 1), (yResolution - 1)];

        normalOfFaces = new Vector3[(xResolution - 1), (yResolution - 1)];

        gradient = new float[xResolution, yResolution];

        terrainSeed = Random.Range(0, 50);
        scale = Random.Range(8f, 30f);
        //scale = 30;
        persistance = Random.Range(0.2f, 0.7f);
        lacunarity = Random.Range(1f, 2f);
        offset = new Vector3( Random.Range(-500f, 500f), 0f, Random.Range(-500f, 500f) );
        noise = Noise.GenerateNoiseMap(xResolution, yResolution, terrainSeed, scale, 4, persistance, lacunarity, offset, noiseIndex);

        Debug.Log("Scale: "+scale);
        Debug.Log("Persistance: "+persistance);
        Debug.Log("Lacunarity: "+lacunarity);

        

        fillRawVertices();
        OffsetVertices();
        MakeNoiseMask();
        Random.seed = System.DateTime.Now.Millisecond;
        AddHeights();
        CreateShape();
               
    }


    void fillRawVertices(){
        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){
                rawVertices[x,y] = new Vector3(x * xTileLength, 0f, y * yTileLength);
                
            }
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
                }
                
            }
        }
    }

    

    void AddHeights(){
        //offset += new Vector3(0.1f, 0f, 0.1f)*Time.deltaTime;
        
        minHeight = Mathf.Infinity;
        peakHeight = 0;
        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){

                float yDist = Mathf.PerlinNoise(rawVertices[x,y].x * 0.3f, rawVertices[x,y].z * 0.3f ) * 1f;
                rawVertices[x,y] += Vector3.up * noise[x,y] * 35f;  //+ Vector3.up*yDist;
                
                if(rawVertices[x,y].y > peakHeight)
                    peakHeight = rawVertices[x,y].y;

                if(rawVertices[x,y].y < minHeight)
                    minHeight = rawVertices[x,y].y;

            }
        }

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

    bool DetectVertexFrameExit(){
        int count = 0;
        for(int y = 0; y < yResolution; y++){
            
            if(rawVertices[1,y].x < -0f){
                count++;
            }
        }
        if(count == yResolution){
            return true;
        }
        else{
            return false;
        }
    }

    void ShiftRawVerticesByTwoColumn(){
        float offsetXLimit = offsetPercent * xTileLength/2f;
        float offsetYLimit = offsetPercent * yTileLength/2f;
        Random.seed = System.DateTime.Now.Millisecond;


        for (int y = 0; y < yResolution; y++) {
            for (int x = 0; x < xResolution-2; x++) {
                rawVertices[x,y] = rawVertices[x+2,y];
                //rawVertices[x+1,y] = rawVertices[x+2,y];
            }
            rawVertices[xResolution - 1,y] = new Vector3((xResolution-1) * xTileLength, 0f, y * yTileLength);
            rawVertices[xResolution - 2,y] = new Vector3((xResolution-2) * xTileLength, 0f, y * yTileLength);

            rawVertices[xResolution-1,y] += new Vector3(Random.Range(-offsetXLimit, offsetXLimit), 0f, Random.Range(-offsetYLimit, offsetYLimit));
            rawVertices[xResolution-2,y] += new Vector3(Random.Range(-offsetXLimit, offsetXLimit), 0f, Random.Range(-offsetYLimit, offsetYLimit));
        }
    }

    void ShiftNoiseByOneColumn(){
        for (int y = 0; y < yResolution; y++) {
            for (int x = 0; x < xResolution-1; x++) {
                noise[x,y] = noise[x+1,y];
            }
        }
    } 

    // Update is called once per frame
    protected void Update()
    {
        minHeight = Mathf.Infinity;
        peakHeight = 0;
        for(int y = 0; y < yResolution; y++){
            for(int x = 0; x < xResolution; x++){
                rawVertices[x,y] = rawVertices[x,y] - Vector3.up*rawVertices[x,y].y - new Vector3(1f, 0f, 0f)*Time.deltaTime*terrainMovementSpeed;

                
                rawVertices[x,y] += Vector3.up * noise[x,y] * 35f;
                
                if(rawVertices[x,y].y > peakHeight)
                    peakHeight = rawVertices[x,y].y;

                if(rawVertices[x,y].y < minHeight)
                    minHeight = rawVertices[x,y].y;
                
            }
        }


        
        if(DetectVertexFrameExit()){
            ShiftRawVerticesByTwoColumn();

            noiseIndex += 2;
            noise = Noise.GenerateNoiseMap(xResolution, yResolution, terrainSeed, scale, 4, persistance, lacunarity, offset, noiseIndex);

        }


        CreateShape();

    }
}
