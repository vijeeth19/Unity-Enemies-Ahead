using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TankEnemy : Enemy
{
    
    
    public float startingHealth = 50;
    //public Image healthBar;
    public GameObject deathEffect;
    public event Action deathEvent;
    public event Action enemyPassedEvent;
    public LayerMask tankMask;

    WallSpawner ws;
    BrickWall bw;

    Animator animator;
    AnimatorStateInfo currInfo;

    float animatorIdleNumber;
    float animatorSpeedPercent;

    protected override void Start()
    {
        base.Start();
        //initialMovementSpeed = 3f;
        movementSpeed = initialMovementSpeed;
        SetSpeed(movementSpeed);
        health = startingHealth;
        deathEvent += FindObjectOfType<EnemySpawner>().tankSpawner.OnEnemyDeath;
        deathEvent += FindObjectOfType<CoinSpawner>().ToSpawnCoin;
        enemyPassedEvent += FindObjectOfType<EnemySpawner>().tankSpawner.OnEnemyPassed;
        enemyPassedEvent += FindObjectOfType<PlayerMovement>().OnGeneralEnemyPassed;
        ws = FindObjectOfType<WallSpawner>();
        bw = FindObjectOfType<BrickWall>();

        animator = GetComponent<Animator>();
        currInfo = animator.GetCurrentAnimatorStateInfo(0);

        animatorIdleNumber = (float) UnityEngine.Random.Range(0, 2);
        SetAnimationStates(false, false);
        enemyType = "Tank";
    }

    public override void TakeHit(float damage, Vector3 hitPoint){
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
        
        if(transform.position.z < -4f){
            if(enemyPassedEvent != null){
                enemyPassedEvent();
            }
            base.OutOfBoundsDestroy();
        }
        
    }

    bool detectTankAhead(float distance, Vector3 origin){
        Ray ray = new Ray(origin, Vector3.back);
        RaycastHit hit;
        Debug.DrawLine(origin, origin + Vector3.back, Color.blue);
        if(Physics.Raycast(ray, out hit, distance, tankMask)){
            
            return true;
        }
        else{
            return false;
        }
    }

    

    bool DetectWallAhead(float distance, out RaycastHit hit, Vector3 origin){

        Vector3 surfaceNormal = CalculateAverageNormalVector(transform.position.x, transform.position.z);
        Vector3 vectorA = LinearAlgebra.FindNormalVectors(surfaceNormal)[0];
        Vector3 vectorB = LinearAlgebra.FindNormalVectors(surfaceNormal)[1];
        Vector3 forwardVector;
        
        if(vectorA.x == 0){
            forwardVector = vectorA;
        }
        else if(vectorB.x == 0){
            forwardVector = vectorB;
        }
        else{
            float scalingFactor = vectorB.x / vectorA.x;

            Debug.Log(vectorA + " - " + vectorB + scalingFactor);

            forwardVector = vectorA - scalingFactor*vectorB;

        }
        

        forwardVector = (forwardVector.z > 0) ? -1*forwardVector : forwardVector;

        Vector3 rayOrigin = origin - 0.75f*surfaceNormal;

        Ray ray = new Ray(rayOrigin, forwardVector.normalized);

        //Debug.Log(transform.position + " " + transform.position + forwardVector*5f);
        Debug.DrawLine(rayOrigin, rayOrigin + forwardVector.normalized*distance);

        if(Physics.Raycast(ray, out hit, distance, wallMask)){
            Debug.DrawLine(rayOrigin, rayOrigin + forwardVector.normalized*distance, Color.red);
            return true;
        }
        else{
            return false;
        }
    }

    IEnumerator RemoveFromArrayList(int targetIndex){
        yield return new WaitForSeconds(0.5f);
        ws.wallPositionData.RemoveAt(targetIndex);
    }

    void SetAnimationStates(bool isDestroying, bool isIdle){
        animator.SetBool("isDestroying", isDestroying);
        animator.SetBool("isIdle", isIdle);
    }

    // Update is called once per frame
    void Update()
    {
        if(!exploded){
        movement();
        OutOfBoundsDestroy();

        healthBar.fillAmount = health/startingHealth;

        RaycastHit[] hitArray = new RaycastHit[5];
        RaycastHit hit;

        float[] spawnPositionX = new float[] {-5f,-3f,-1f,1f,3f,5f};
        float snapXPos = 0f;
        float minDistX = 1000f;
        for(int i = 0; i < 6; i++){
            float dist = Mathf.Abs(spawnPositionX[i] - transform.position.x);
             
            if(dist < minDistX){
                minDistX = dist;
                snapXPos = spawnPositionX[i];
            }
        }


        

        for(int i = 0; i < 5; i++){
            if(DetectWallAhead(4f, out hit, transform.position + Vector3.right*(-transform.position.x + snapXPos-0.8f + 0.4f * i))){
                Debug.Log("SnapX: " + snapXPos);
                float[] removalWallData = new float[2];
                
                
                int listIndex = 0;
                int targetIndex = -1;

                if(i == 2){
                    removalWallData[0] = snapXPos;
                    removalWallData[1] = ws.SnapWall(snapXPos, hit.point.z)[1];

                    foreach(float[] array in ws.wallPositionData){
                        if(array[0] == removalWallData[0] && array[1] == removalWallData[1]){
                            targetIndex = listIndex;
                        }
                        listIndex ++;
                    }
                }
                
                

                
                IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
                if(damageableObject != null){
                    
                    SetAnimationStates(true, false);
                    currInfo = animator.GetCurrentAnimatorStateInfo(0);
                    SetSpeed(0f);
                    if(currInfo.normalizedTime > 0.5f && currInfo.IsName("Destroying")){
                        
                        SetSpeed(movementSpeed);
                        damageableObject.TakeHit(10f, Vector3.zero);
                        animatorSpeedPercent = 0f;  
                        SetAnimationStates(false, false);


                        if(i == 2 ){
                            if(hit.collider.GetComponent<BrickWall>() != null){
                                if(hit.collider.GetComponent<BrickWall>().instanceBuildStatus == BrickWall.buildStatus.Built){
                                    int arrayIndex;
                                    switch(removalWallData[0]){
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

                                    ws.columnArrays[arrayIndex].Remove(removalWallData[1]);
                                }
                            }                 
                            

                        }


                        if(targetIndex != -1)
                            //StartCoroutine(RemoveFromArrayList(targetIndex));
                            ws.wallPositionData.RemoveAt(targetIndex);
                        
                        if(i==2){
                            BrickWall brickwall = (BrickWall) damageableObject;
                            brickwall.UnblockNearblyWallPlacement();
                        }

                    }

                    /*
                    damageableObject.TakeHit(10f, Vector3.zero);
                    animatorSpeedPercent = 0f;  
                    SetAnimationStates(true, false);
                    
                    if(i==2){
                        BrickWall brickwall = (BrickWall) damageableObject;
                        brickwall.UnblockNearblyWallPlacement();
                    }
                    */

                }else{
                    //if(detectTankAhead(1f, transform.position + Vector3.up * 2f)){
                        SetSpeed(0f);
                        animatorSpeedPercent = animatorIdleNumber;
                        SetAnimationStates(false, true);
                        
                        animator.SetFloat("speedPercent", animatorIdleNumber);
                    //}
                    

                }

                
                
            }
            else{

                if(i == 2){
                    if(detectTankAhead(1f, transform.position + Vector3.up * 2f)){
                        SetSpeed(0f);
                        animatorSpeedPercent = animatorIdleNumber;
                        SetAnimationStates(false, true);
                        
                        animator.SetFloat("speedPercent", animatorIdleNumber);
                    }else{
                        SetSpeed(movementSpeed);
                        animatorSpeedPercent = 3f;
                        SetAnimationStates(false, false);
                    }
                }
                
                
            }
        }

        //animator.SetFloat("speedPercent", animatorSpeedPercent);
        //Debug.Log(animatorSpeedPercent + "  " + animatorIdleNumber);
        transform.position -= Vector3.up;
        }
    }
}
