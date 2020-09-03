using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    
    public enum buildStatus {Building, Built};
    public buildStatus instanceBuildStatus;

    public bool buildable;

    public Image lifeBar;
    protected float life;
    protected float startingLife;
    protected float deathRate;
    protected int cost;

    protected MeshGenerator mg;
    protected WallSpawner ws;
    public EnemySpawner es;
    public PlayerMovement pm;


    protected void Start()
    {
        instanceBuildStatus = buildStatus.Building;
        
        es = FindObjectOfType<EnemySpawner>();

        mg = FindObjectOfType<MeshGenerator>();
        ws = FindObjectOfType<WallSpawner>();
        pm = FindObjectOfType<PlayerMovement>();
    }

    protected void SetStartingLife(float startingLife){
        this.startingLife = startingLife;
        life = startingLife;
    }

    protected void SetDeathRate(float deathRate){
        this.deathRate = deathRate;
    }

    protected void SetCost(int cost){
        this.cost = cost;
    }

    public int GetCost(){
        return cost;
    }

    public void UnblockNearblyWallPlacement(){
        int totalRows = mg.meshTileData.GetLength(0);
        int totalCols = mg.meshTileData.GetLength(1);

        for(int i = 0; i<totalRows; i++){
            for(int j = 0; j<totalCols; j++){
                mg.meshTileData[i,j] = true;
            }
        }

        foreach(float[] array in ws.wallPositionData){
            if(array[0] != 0f && array[1] != 0f)
                ws.BlockNearbyWallPlacement(array[0], array[1]);
        }
    }

    void Age(){
        life -= deathRate * Time.deltaTime;
        lifeBar.fillAmount = life/startingLife;
        if(life <= 0){
            Destroy();
        }
    }

    void Destroy(){
        /*
        1. Destroys the gameObject
        2. Removes from ws.columnArrays
        3. Removes from ws.wallPositionData
        4. Unblocks nearby wall placement
        */

        Debug.Log("BOOOM");
        Destroy(gameObject);

        int arrayIndex = (int) ((transform.position.x + 5) / 2);

        ws.columnArrays[arrayIndex].Remove(transform.position.z);

        float[] removalWallData = new float[2];
        removalWallData[0] = transform.position.x;
        removalWallData[1] = transform.position.z;

        //FindObjectOfType<WallSpawner>().wallPositionData.RemoveAt(FindObjectOfType<WallSpawner>().wallPositionData.Count - 1);
        int listIndex = 0;
        int targetIndex = -1;
        foreach(float[] array in ws.wallPositionData){
            if(array[0] == removalWallData[0] && array[1] == removalWallData[1]){
                targetIndex = listIndex;
            }
            listIndex ++;
        }

        if(targetIndex != -1)
            ws.wallPositionData.RemoveAt(targetIndex);

        UnblockNearblyWallPlacement();
    }

    // Update is called once per frame
    protected void Update()
    {
        if(instanceBuildStatus == buildStatus.Built && !es.openNextWave && !pm.playerDead){
            Age();
            GetComponent<Collider>().enabled = true;
        }
        else{
            GetComponent<Collider>().enabled = false;
        }
    }
}
