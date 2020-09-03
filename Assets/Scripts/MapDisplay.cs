using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;
    float[,] noise, gradient;
    public LowPolyTerrain lpt;

    void Start(){
        lpt.Start();
        noise = Noise.GenerateNoiseMap(100, 100, 21, 25f, 4, 0.5f, 1.87f, Vector3.right * 13f + Vector3.forward * 10f, 0);
        DrawNoiseMap(lpt.gradient);
        
    
    }

    void MakeGradient(){
        float gradientRate = 0.25f;
        for(int y = 0; y < 140; y ++){
            for(int x = 0; x < 100; x++){
                gradient[x,y] = 0f;
                if(x >= 50){
                    gradient[x,y] = Mathf.Clamp01(gradientRate * ((x-50)-6));
                }
                if(x <50){
                    gradient[x,y] = Mathf.Clamp01(-gradientRate * ((x-50)+6));
                }
                if(y <= 120){
                    gradient[x,y] += Mathf.Clamp01( gradientRate * ((120-y)-80));
                }
                if(y > 120){
                    gradient[x,y] += Mathf.Clamp01(-gradientRate * (120-y));
                }
                gradient[x,y] = Mathf.Clamp01(gradient[x,y]);
            }
        }
    }

	public void DrawNoiseMap(float[,] noiseMap) {
		int width = noiseMap.GetLength (0);
		int height = noiseMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
			}
		}
		texture.SetPixels (colourMap);
		texture.Apply ();

		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (width, 1, height);
	}
	
}
