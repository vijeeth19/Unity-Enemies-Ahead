using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DodgerEnemy : Enemy
{
    //enemy properties
    public float startingHealth = 20;
    //public Image healthBar;
    public GameObject deathEffect;
    public event Action deathEvent;
    public event Action enemyPassedEvent;

    //following properties are required for making the laser scanner
    int totalPoints = 100; //100 original
    Vector3 testVector;
    float angleRange = Mathf.PI * 4 / 3;
    float angleIncrement;
    Vector3[] circularData;
    float scanRadius = 6f; //6f original
    int preferredSide;

    //following properties are required to deal with local minimum points (Stuck scenario)
    int pastPosCount = 15;
    Queue<Vector3> pastPosVectors = new Queue<Vector3>();
    bool stuckCurrently = false;
    float stuckAngle = 0f;
    int stuckFrame = 0;
    
    Array[] arr;
    public enum state{LongMove, VeerOffDirection, ShortMove, CorrectDirection};
    public state currentState;
    public int[] pointers;

    float destColumnX;

    Animator animator;

    protected override void Start()
    {
        base.Start();
        health = startingHealth;
        
        //initialMovementSpeed = 5f;
        movementSpeed = initialMovementSpeed;
        SetSpeed(movementSpeed);

        angleIncrement = angleRange/(totalPoints-1);
        testVector = new Vector3 (0f, 1f, 0f);

        circularData = new Vector3[totalPoints];
        UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
        preferredSide = (int) Mathf.Pow(-1,UnityEngine.Random.Range(0,2));
        preferredSide = (transform.position.x == -5f) ? -1: (transform.position.x == 5f) ? 1: preferredSide;
        Debug.Log("Spawn X" + transform.position.x + "preffered side: " + preferredSide);
        //initialize the Queue
        for(int i = 0; i < pastPosCount; i++ ){
            pastPosVectors.Enqueue(Vector3.zero);
        }

        deathEvent += FindObjectOfType<EnemySpawner>().dodgerSpawner.OnEnemyDeath;
        deathEvent += FindObjectOfType<CoinSpawner>().ToSpawnCoin;
        enemyPassedEvent += FindObjectOfType<EnemySpawner>().dodgerSpawner.OnEnemyPassed;
        enemyPassedEvent += FindObjectOfType<PlayerMovement>().OnGeneralEnemyPassed;

        arr = new Array[FindObjectOfType<WallSpawner>().columnArrays.Length];
        pointers = new int[arr.Length];
        for(int i = 0; i < FindObjectOfType<WallSpawner>().columnArrays.Length; i++){
            FindObjectOfType<WallSpawner>().columnArrays[i].WallUpdated += this.OnWallUpdated;
            arr[i] = FindObjectOfType<WallSpawner>().columnArrays[i];
            pointers[i] = 0;
        }

        currentState = state.LongMove;

        animator = GetComponent<Animator>();
        enemyType = "Dodger";
        
    }

    public override void TakeHit(float damage, Vector3 hitPoint){
        Debug.Log("dodger took hit");
        if (damage >= health && !dead) {
			Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.identity) as GameObject, 2f);
		}
		base.TakeHit (damage, hitPoint);

    }
    
    public override void Die(){
        base.Die();
        if(deathEvent != null){
            deathEvent();
        }
    }

    public override void OutOfBoundsDestroy(){
        base.OutOfBoundsDestroy();
        if(transform.position.z < 1f){
            if(enemyPassedEvent != null){
                enemyPassedEvent();
            }
        }
        
    }

    void DrawCircle(Vector3 vectorA, Vector3 vectorB, float radius){
        Vector3 nextPoint;
        

        for( int i = 0; i<totalPoints; i++ ){
            float nextAngle = -1* angleRange/2 + i * angleIncrement;
            nextPoint = radius * Mathf.Cos(nextAngle) * vectorA + radius * Mathf.Sin(nextAngle) * vectorB;
            circularData[i] = nextPoint;
        }
    }

    float[] ScanSurroundings(Vector3 origin, float radius){
        float[] distances = new float[totalPoints];
        RaycastHit hit;

        for(int i = 0; i<totalPoints; i++ ){
            Vector3 endPoint = origin + circularData[i];
            Ray ray = new Ray(origin, (endPoint - origin).normalized);
            if(Physics.Raycast(ray, out hit, radius, wallMask, QueryTriggerInteraction.Collide)){
                distances[i] = (hit.point - origin).magnitude;


                Debug.DrawLine(origin, hit.point);
                
            }
            else{
                distances[i] = radius;
                Debug.DrawLine(origin, origin + circularData[i], Color.red);
            }
            
        }

        return distances;
    }

    int FindRotationAngleIndex(float[] distArray){
        float sumDist = 0;
        float maxSumDist = 0;
        int maxAngleIndex = 0;
        List<float> maxSumDistAngles = new List<float>();

        int integerWidth = 15;
        int iStart = (integerWidth - 1)/2; //4
        int iEnd = distArray.Length - iStart; //distArray.Length-4
        int jStart = -1 * iStart;
        int jEnd = iStart + 1;

        //Find the maximum sum of distance across integerWidth elements
        for(int i = iStart; i< iEnd ; i++ ){
            sumDist = 0;
            for( int j = jStart ; j < jEnd ; j++ ){
                sumDist += distArray[i+j];
            }
            
            if(sumDist > maxSumDist){
                maxSumDist = sumDist;
                maxAngleIndex = i;
            }
        }

        
        //List all the angles with maximum sum of distance 
        for(int i = iStart; i< iEnd ; i++ ){
            sumDist = 0;
            for( int j = jStart ; j < jEnd ; j++ ){
                sumDist += distArray[i+j];
            }
            if(sumDist == maxSumDist){
                maxSumDistAngles.Add(-1*angleRange/2 + i*angleIncrement);
            }
        }


        //Find best angle out of the listed angles -- CONDITIONAL
        float bestAngle;
        if(!stuckCurrently){
            bestAngle = Mathf.PI;
            stuckAngle = 0;
            foreach(float el in maxSumDistAngles){
                if(Mathf.Abs(el) < Mathf.Abs(bestAngle)){
                    bestAngle = el;
                }
                if(Mathf.Abs(el) < Mathf.Abs(bestAngle)){
                    stuckAngle = el;
                }
            }
        }
        else{
            int stuckAngleIndex = (int)((stuckAngle+angleRange/2)/angleIncrement);
            if( distArray[stuckAngleIndex] > 1f){
                //Apply brute force
                //Debug.Log("APPLYING FORCE!!");
                bestAngle = stuckAngle;
                stuckFrame ++;

                //move for 25 framess
                if(stuckFrame > 200){
                    stuckCurrently = false;
                }
            }
            else{
                //leave stuck mode
                stuckCurrently = false;

                //still set best angle = stuckAngle for one last time
                bestAngle = stuckAngle; 
            }

            
        }
        

        //Check if the best angle is bi-directional
        int biDirectionalCount = 0;
        foreach(float el in maxSumDistAngles){
            if(Mathf.Abs(el) == Mathf.Abs(bestAngle)){
                biDirectionalCount ++;
            }

        }

        //Randomize angle direction if best angle is bi-directional
        bestAngle = (biDirectionalCount==2) ? preferredSide*bestAngle : bestAngle;

        int angleIndex = (int)((bestAngle+angleRange/2)/angleIncrement);

        return angleIndex;
    }

    Vector3 FindRange(){
        float maxX = 0f;
        float maxY = 0f;
        float maxZ = 0f;
        float minX = 100000f;
        float minY = 100000f;
        float minZ = 100000f;

        foreach (var j in pastPosVectors.ToArray()){
            maxX = (j.x > maxX) ? j.x : maxX;
            maxY = (j.y > maxY) ? j.y : maxY;
            maxZ = (j.z > maxZ) ? j.z : maxZ;

            minX = (j.x < minX) ? j.x : minX;
            minY = (j.y < minY) ? j.y : minY;
            minZ = (j.z < minZ) ? j.z : minZ;

        }

        return new Vector3((maxX - minX) , (maxY - minY) , (maxZ - minZ));
    }

    public void UpdatePointers(){
        
        for(int i = 0; i<pointers.Length; i++){
            if(transform.position.z < arr[i].Peek(pointers[i])){
                pointers[i]++;
            }
        }
        
        
    }

    public void OnWallUpdated(object source, WallEventArgs wall){
        //Debug.Log("Recognized Wall Updated " + wall.column + "  " + wall.z + " " + wall.wallAdded);
        //Debug.Log("Pointed wall " + arr[wall.column].Peek(pointers[wall.column]));
        if(wall.wallAdded){
            if(wall.z > arr[wall.column].Peek(pointers[wall.column])){
                pointers[wall.column]++;
            }
        }
        if(!wall.wallAdded){
            //Debug.Log("reached here");
            if(wall.z > arr[wall.column].Peek(pointers[wall.column])){
                //Debug.Log("reached inside, pointers[wall.column] : " + pointers[wall.column]);
                pointers[wall.column]--;
                //Debug.Log("after subtraction, pointers[wall.column] : " + pointers[wall.column]);
            }
        }
        
    }

    public void StateMachine(){

        int arrayIndex;
        switch(transform.position.x){
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

        switch(currentState){
            case state.LongMove:
                currentState = (arr[arrayIndex].Peek(pointers[arrayIndex]) != -100f) ? ((transform.position.z - arr[arrayIndex].Peek(pointers[arrayIndex]) < 3f) ? state.VeerOffDirection: state.LongMove) : state.LongMove;
                break;
            case state.VeerOffDirection:
                Debug.Log(transform.rotation.eulerAngles.y);
                float rotY1 = (transform.rotation.eulerAngles.y < 180f) ? transform.rotation.eulerAngles.y: transform.rotation.eulerAngles.y - 360f;
                currentState = (Mathf.Abs(rotY1) < 45f) ? state.VeerOffDirection: state.ShortMove;
                break;
            case state.ShortMove:
                currentState = (-1*preferredSide * destColumnX + transform.position.x * preferredSide > 0f) ? state.ShortMove: state.CorrectDirection;
                break;
            case state.CorrectDirection:
                Debug.Log(Mathf.Sign(transform.rotation.eulerAngles.y));
                Debug.Log(transform.rotation.eulerAngles.y);
                float rotY = (transform.rotation.eulerAngles.y < 180f) ? transform.rotation.eulerAngles.y: transform.rotation.eulerAngles.y - 360f;
                currentState = ( Mathf.Sign(rotY) == preferredSide ) ? state.CorrectDirection : state.LongMove;

                if(currentState == state.LongMove){
                    UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
                    preferredSide = (int) Mathf.Pow(-1,UnityEngine.Random.Range(0,2));
                    preferredSide = (transform.position.x == -5f) ? -1: (transform.position.x == 5f) ? 1: preferredSide;
                }
                break;
            default: 
                break;
        }

        switch(currentState){
            case state.LongMove:
                //Debug.Log("Long move");
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.Translate((-1*transform.forward) * speed * Time.deltaTime );
                destColumnX = transform.position.x - preferredSide*2f;
                animator.SetBool("transition", false);
                break;
            case state.VeerOffDirection:
                //Debug.Log("Veer off direction");
                transform.Translate((-1*transform.forward) * 0f * Time.deltaTime );

                Quaternion lookRotation1 = Quaternion.Euler(0f,preferredSide*45f,0f);
                Vector3 rotation1 = Quaternion.Lerp(transform.rotation, lookRotation1, speed/5f * Time.deltaTime).eulerAngles;
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + speed * preferredSide, 0f);
                animator.SetBool("transition", true);
                break;
            case state.ShortMove:
                //Debug.Log("Short move");
                transform.rotation = Quaternion.Euler(0f, 45f * preferredSide, 0f);
                transform.Translate((-1*transform.forward) * speed * Time.deltaTime );
                animator.SetBool("transition", true);
                break;
            case state.CorrectDirection:
                transform.position += Vector3.right * (destColumnX - transform.position.x);
                
                Quaternion lookRotation = Quaternion.Euler(0f,0f,0f);
                Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, speed/5f * Time.deltaTime).eulerAngles;
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y - speed * preferredSide, 0f);
                animator.SetBool("transition", true);
                //Debug.Log("Correct Direction");
                break;
            default: 
                break;
        }
    }

    void Update()
    {
        //movement();
        //transform.Translate((-1*transform.forward) * 2.1f * Time.deltaTime );
        if(!exploded){
        OutOfBoundsDestroy();
        SetSpeed(movementSpeed);
        //Debug.Log("before update pointers" + " " + pointers[0]+" "+pointers[1]+" "+pointers[2]+" "+pointers[3]+" "+pointers[4]+" "+pointers[5]);
        UpdatePointers();
        StateMachine();

        healthBar.fillAmount = health/startingHealth;

        float newY = -1000f;
        Vector3 rayOrigin = new Vector3(transform.position.x , 50f, transform.position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, terrainMask)){
            newY = hit.point.y;
        }

        transform.position += Vector3.up*(0.3f - transform.position.y) ; 

        /*
        List<Vector3> vectorList = LinearAlgebra.FindNormalVectors(testVector);
        Vector3 vectorA = vectorList[0];
        Vector3 vectorB = vectorList[1];
        Vector3 normalScanner = CalculateAverageNormalVector( transform.position.x, transform.position.z );
        testVector = normalScanner;

        DrawCircle(vectorA, vectorB, scanRadius);
        float[] distances = ScanSurroundings( transform.position - 0.75f*normalScanner, scanRadius);

        int angleIndex= FindRotationAngleIndex(distances);

        pastPosVectors.Dequeue();
        pastPosVectors.Enqueue(transform.position);

        
        if(FindRange().z < 0.5f){
            stuckFrame = 0;
            stuckCurrently = true;
            //Debug.Log("STUCK!!");
        }
            

        Vector3 targetPoint = circularData[angleIndex];
        Debug.DrawLine(transform.position, transform.position + targetPoint, Color.green);
        */
        
        //transform.rotation = Quaternion.FromToRotation( (-1 *transform.forward).normalized, targetPoint.normalized);

        if(transform.position.z < 0){
            Destroy(gameObject);
        }

        //Debug.Log(this + " " + pointers[0]+" "+pointers[1]+" "+pointers[2]+" "+pointers[3]+" "+pointers[4]+" "+pointers[5]);
        }
    }
}
