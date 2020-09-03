using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallSpawner : MonoBehaviour
{

    public Camera camera;
    public BrickWall wallColumnPrefab;
    public Turret turretPrefab;
    public Turret turretInstance;
    public BrickWall wallPrefab;

    public LayerMask mask;
    public LayerMask shootButtonLayer;
    BrickWall[] wallColumns = new BrickWall[5]; //Contains the gameObjects of the 5 columns of the wall to be placed next
    public List<float[]> wallPositionData = new List<float[]>(); //Each element in this list contains info about X and Z positions of every wall
    public float wallPlacementZExtremum = 11.5f;
    public bool buildable = true;
    bool moveable = true;
    public List<BrickWall[]> walls = new List<BrickWall[]>(); //Each element is a wall, where a wall is an array of wallColumns gameobjects
    float wallPosX, wallPosZ;
    public MeshRenderer tileMask;

    public enum buildState {NotBuilding, StartedBuilding, DuringBuilding, FinishedBuilding};
    public buildState currentState;
    
    public int buildChoice; //0 for wall and 1 for turret 

    
    public Array[] columnArrays;
    private int columnCount = 6;

    public enum State  {NotBuilding, NotStartedBuilding, StartedBuilding, FinishedBuilding, DuringBuilding, IllegalTouch};
    public State state;
    
    Building wallToBuildPrefab, wallToBuildInstance;

    IngameUI igui;
    //public Animator illegalTouchAnimator;
    //bool illegalTouchAnimatorBool = false;
    CoinManager cm;

    public Building[] turretArray;
    public Inventory inv;
    public ShopUI shop;
    public EnemySpawner es;
    public PlayerMovement pm;
    public CameraAnimation ca;

    // Start is called before the first frame update
    void Start()
    {
        currentState = buildState.NotBuilding;
        wallPosX = -1000f;
        wallPosZ = -1000f;

        buildChoice = 0;
        
        for(int i = 0; i < 5; i++){
            wallColumns[i] = null;
        }

        columnArrays = new Array[columnCount];

        for(int i = 0; i < columnCount; i++){
            columnArrays[i] = new Array(9, i);
        }

        igui = FindObjectOfType<IngameUI>();

        state = State.NotBuilding;
        //illegalTouchAnimator.SetBool("ShowText",false);

        cm = FindObjectOfType<CoinManager>();
    }



    void inputHandler(){
        if(FindObjectOfType<IngameUI>().playmode != IngameUI.Playmode.Shoot){
            
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition + Vector3.up*touchShiftUp);
            if(Physics.Raycast(ray, out hit, 100f, mask)){
                if(Input.GetMouseButtonDown(0)){
                    currentState = buildState.StartedBuilding;
                }
                else if(Input.GetMouseButton(0)){
                    currentState = buildState.DuringBuilding;
                }
                else if(Input.GetMouseButtonUp(0)){
                    currentState = buildState.FinishedBuilding;
                }
                else{
                    currentState = buildState.NotBuilding;
                }

                if(FindObjectOfType<IngameUI>().buildchoice == IngameUI.BuildChoice.Wall){
                    buildChoice = 0;
                } 
                if(FindObjectOfType<IngameUI>().buildchoice == IngameUI.BuildChoice.Turret){
                    buildChoice = 1;
                }
            }
        }
               
    }

    public float[] SnapWall(float xPosMouse, float zPosMouse){
        float startingZ = 11.5f;
        float endingZ = 70f;
        float zSpace = 1f;
        int totalNum = (int) ((endingZ - startingZ)/zSpace);
        float[] spawnPositionX = new float[] {-5f,-3f,-1f,1f,3f,5f};
        float[] spawnPositionZ = new float[totalNum];
        float minDistX = 100f;
        float snapXPos = 0f;
        float minDistZ = 1000f;
        float snapZPos = 0f;

        for(int i = 0; i < totalNum; i++){
            spawnPositionZ[i] = startingZ + i;
        }

        for(int i = 0; i < 6; i++){
            float dist = Mathf.Abs(spawnPositionX[i] - xPosMouse);
             
            if(dist < minDistX){
                minDistX = dist;
                snapXPos = spawnPositionX[i];
            }
        }

        for(int i = 0; i < totalNum; i++){
            float dist = Mathf.Abs(spawnPositionZ[i] - zPosMouse);

            if(dist < minDistZ){
                minDistZ = dist;
                snapZPos = spawnPositionZ[i];
            }
        }

        float[] wallXZ = new float[2];
        wallXZ[0] = snapXPos;
        wallXZ[1] = snapZPos;

        return wallXZ;
    }

    float[,] FindWallColumnHeights (float wallCenterX, float wallCenterZ){
        float[,] wallInfo = new float [2,5]; //first row contains the x-values of the columns, and second row contains the y value of the columns
        RaycastHit hit;
        

        for(int i = 0; i < 5; i++){
            
            float columnCenterX = (wallCenterX - 0.8f) + 0.4f*i;
            wallInfo[0,i] = columnCenterX;
            wallInfo[1,i] = -1000f; //Initializes the array, if the final returned array has a -1000 then ray didnt hit the terrain


            Vector3 origin = new Vector3(columnCenterX, 100f, wallCenterZ);
            Vector3 endPoint = new Vector3(columnCenterX, -100f, wallCenterZ);
            Ray ray = new Ray(origin, (endPoint-origin).normalized);
            if(Physics.Raycast(ray, out hit, 1000f, mask)){
                wallInfo[1,i] = hit.point.y + 0.3f;
            }

        }

        
        
        return wallInfo;
    }

    //Mode 0 adds an element, while mode 1 edits the last element
    void StoreWallData(float wallCenterX, float wallCenterZ, int mode){
        float[] wallData = new float[2];
        wallData[0] = wallCenterX;
        wallData[1] = wallCenterZ;
        if(mode == 0){
            wallPositionData.Add(wallData);
        }
        else if(mode == 1){
            wallPositionData[wallPositionData.Count - 1] = wallData;

            int arrayIndex;
            switch(wallCenterX){
                case -5f:
                    arrayIndex = 0;
                    break;
                case -3f:
                    arrayIndex = 1;
                    break;
                case -1f:
                    arrayIndex = 2;
                    break;

                case 1f:
                    arrayIndex = 3;
                    break;
                case 3f:
                    arrayIndex = 4;
                    break;

                case 5f:
                    arrayIndex = 5;
                    break;   
                default: 
                    arrayIndex = 100;  
                    break;           
            }

            columnArrays[arrayIndex].Add(wallCenterZ);
        }

        
    }

    bool AllowWallPlacement(float wallToPlaceX, float wallToPlaceZ){

        bool insideGameArea = true;
        bool placableTile = true;
        int indexX = (int)((6f + wallToPlaceX - 1f)/2f); 
        int indexZ = (int)(-11f + wallToPlaceZ - 0.5f);

        //if theres no walls originally Allow placement
        if(wallPositionData.Count == 0 && wallToPlaceZ > wallPlacementZExtremum){
            Debug.Log("No walls yet - true");
            return true;
        }
        else{
            //if there are walls in the scene ...

            if(wallToPlaceZ < wallPlacementZExtremum){
                Debug.Log("Outside game");
                insideGameArea = false;
            }

            placableTile = FindObjectOfType<MeshGenerator>().meshTileData[indexX, indexZ];

            return insideGameArea && placableTile;
        }
        

    }

    //This function sets the neighboring values of wall in meshTileData to be false, disabling mesh creations in these areas
    public void BlockNearbyWallPlacement(float centerX, float centerZ){
        int totalRows = FindObjectOfType<MeshGenerator>().meshTileData.GetLength(0);
        int lastRowIndex = totalRows - 1;
        int totalCols = FindObjectOfType<MeshGenerator>().meshTileData.GetLength(1);
        int lastColumnIndex = totalCols - 1;
        
        int zBlockingRadius = 7, xBlockingRadius = 2;

        /*
        For the following section look at the terrain from the top with corner near the left horizontal bounding wall in front of the player
        being the [0,0] in the top left corner of the matrix. The corner near the right horizontal bounding wall in front of the player being
        the [5,0] in the bottom left corner of the matrix. The top right corner of the matrix ([0,~81]) is the leftmost and farthest placable tile.
        The bottom right corner of the matrix ([~81,~81]) is the rightmost and farthest placable tile.
        */

        int indexX = (int)((6f + centerX - 1f)/2f); 
        int indexZ = (int)(-11f + centerZ - 0.5f);
        int availableIndexToUp = 0, availableIndexToDown = 0;
        int availableIndexToLeft = 0, availableIndexToRight = 0;
        
        FindObjectOfType<MeshGenerator>().meshTileData[indexX, indexZ] = false;


        for(int i = 1; i <= xBlockingRadius; i++){
            if(indexX - i >= 0 ){
                availableIndexToUp = i;
            }
            if(indexX + i <= lastRowIndex ){
                availableIndexToDown = i;
            }
        }

        for(int i = 1; i <= zBlockingRadius; i++){
            if(indexZ - i >= 0 ){
                availableIndexToLeft = i;
            }
            if(indexZ + i <= lastColumnIndex ){
                availableIndexToRight = i;
            }
        }

        int startingXValue = indexX - availableIndexToUp;
        int endingXValue = indexX + availableIndexToDown;
        int startingZValue = indexZ - availableIndexToLeft;
        int endingZValue = indexZ + availableIndexToRight;

        for(int z = startingZValue; z <= endingZValue; z++){
            for(int x = startingXValue; x <= endingXValue; x++){
                FindObjectOfType<MeshGenerator>().meshTileData[x, z] = false;
            }
        }
        
    }

    bool IsTouchInside(){
        Ray ray = camera.ScreenPointToRay(Input.mousePosition + Vector3.up*touchShiftUp);
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, mask)){
            if(hit.point.x > -7f && hit.point.x < 7f){
                if(hit.point.z > 5f && hit.point.z < 90f)
                    return true;
            }
        }
        return false;
    }

    void UpdateWallToBuild(){/*
        if(igui.buildchoice == IngameUI.BuildChoice.Wall) wallToBuildPrefab = wallPrefab;
        else wallToBuildPrefab = turretPrefab;*/
        if(inv.hotbarSelectedItem.type == ItemDetails.ItemType.Build){
            switch(inv.hotbarSelectedItem.id){
                case 14: wallToBuildPrefab = turretArray[0]; break;
                case 15: wallToBuildPrefab = turretArray[1]; break;
                case 16: wallToBuildPrefab = turretArray[2]; break;
                case 17:
                case 18:
                case 19:
                    wallToBuildPrefab = turretArray[3]; break;
                case 20:
                case 21:
                case 22:
                    wallToBuildPrefab = turretArray[4]; break;
                case 23:
                case 24:
                case 25:
                    wallToBuildPrefab = turretArray[5]; break;
                case 26:
                case 27:
                    wallToBuildPrefab = turretArray[6]; break;
                default: break;
            }
            
        }
    }
    public int touchShiftUp = 50;
    void StateMachine(){
        switch(state){
            case State.NotBuilding:
                state = (igui.playmode == IngameUI.Playmode.Build) ? State.NotStartedBuilding : State.NotBuilding;
                break;
            case State.NotStartedBuilding:
                if(igui.playmode != IngameUI.Playmode.Build) state = State.NotBuilding;
                else if(Input.GetMouseButtonDown(0)){
                    if(IsTouchInside() && !inv.isInventoryOpen && !shop.isShopOpen && !IsMouseOverUIWithIgnores() && !pm.playerDead && !ca.introduction) state = State.StartedBuilding;
                    else state = State.IllegalTouch;
                }
                else{
                    state = State.NotStartedBuilding;
                }
                break;
            case State.IllegalTouch:
                state = State.NotStartedBuilding;
                break;
            case State.StartedBuilding:
                state = State.DuringBuilding;
                break;
            case State.DuringBuilding:
                state = (!Input.GetMouseButton(0)) ? State.FinishedBuilding: State.DuringBuilding;
                break;
            case State.FinishedBuilding:
                state = State.NotStartedBuilding;
                break;
            default: 
                break;

        }
        switch(state){
            case State.NotBuilding:
                tileMask.enabled = false;
                //Debug.Log("Not Build mode");
                break;
            case State.NotStartedBuilding:
                //illegalTouchAnimatorBool = false;
                //illegalTouchAnimator.SetBool("ShowText",false);
                tileMask.enabled = true;
                //Debug.Log("Not started to build");
                break;
            case State.IllegalTouch:
                //illegalTouchAnimatorBool = true;
                //illegalTouchAnimator.SetBool("ShowText",true);
                //Debug.Log("Illegal touch");
                break;
            case State.StartedBuilding:
                StartedBuilding();
                //Debug.Log("Started Building");
                break;
            case State.DuringBuilding:
                DuringBuilding();
                //Debug.Log("During Building");
                break;
            case State.FinishedBuilding:
                FinishedBuilding();
                //Debug.Log("Finished Building");
                break;
            default:
                break;
        }
    }

    float snappedWallX, snappedWallZ;

    void StartedBuilding(){
        tileMask.enabled = true;
        wallToBuildInstance = Instantiate(wallToBuildPrefab, Vector3.zero, Quaternion.identity);
        if(inv.hotbarSelectedItem.id != 14){
            wallToBuildInstance.GetComponent<Turret>().level = inv.hotbarSelectedItem.level;
            if(inv.hotbarSelectedItem.id != 15) wallToBuildInstance.GetComponent<Turret>().UpdateLevelSettings();
        }
        wallToBuildInstance.instanceBuildStatus = Building.buildStatus.Built;
        StoreWallData(0f, 0f, 0);
    }

    void DuringBuilding(){
        tileMask.enabled = true;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition + Vector3.up*touchShiftUp);
        //If mouse points on the ground
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, mask)){
            snappedWallX = SnapWall(hit.point.x, hit.point.z)[0];
            snappedWallZ = SnapWall(hit.point.x, hit.point.z)[1];
            buildable = AllowWallPlacement(snappedWallX, snappedWallZ);
            Vector3 rayOrigin = new Vector3(snappedWallX, 100f, snappedWallZ);
            Ray rayDown = new Ray(rayOrigin, Vector3.down);
            if(Physics.Raycast(rayDown, out RaycastHit hit2, 200f, mask))
                wallToBuildInstance.transform.position = new Vector3(snappedWallX, hit2.point.y + 0.3f, snappedWallZ);             
        }

    }
    
    void FinishedBuilding(){
        tileMask.enabled = false;  // makes the tileMask mesh invisible when the 'jump' button is not pressed
        if(buildable /*&& (cm.almuns - wallToBuildInstance.GetCost() >= 0)*/){
            wallToBuildInstance.instanceBuildStatus = Building.buildStatus.Built;
            //cm.DebitAlmuns(wallToBuildInstance.GetCost());
            StoreWallData(snappedWallX, snappedWallZ, 1);
            BlockNearbyWallPlacement(snappedWallX, snappedWallZ);
            inv.UpdateItem(inv.hotbarSelectedItem.id, -1);
        }
        else{
            Destroy(wallToBuildInstance.gameObject);
            wallPositionData.RemoveAt(wallPositionData.Count - 1);
        }
        buildable = true;
        
    }

    bool shootClicked = false;
    void OnShootClicked(){
        shootClicked = true;
    }


    void LateUpdate(){

    }

    public bool IsMouseOverUIWithIgnores(){
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for(int i = 0; i < raycastResultList.Count; i++){
            if(raycastResultList[i].gameObject.GetComponent<MouseUIClickThrough>() != null || raycastResultList[i].gameObject.tag == "Click Through"){
                raycastResultList.RemoveAt(i--);
            }
        }
        
        return raycastResultList.Count > 0;
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        
        //inputHandler();
        
        UpdateWallToBuild();
        StateMachine();

        Ray ray = camera.ScreenPointToRay(Input.mousePosition + Vector3.up*touchShiftUp);
        //If mouse points on the shoot button
        if(Physics.Raycast(ray, out RaycastHit hitt, 1000f, shootButtonLayer))
            Debug.Log("button pressed");

    }
}
