using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class JumperEnemy : Enemy
{
    public float startingHealth = 10;
    //public Image healthBar;
    bool whileJump = false;

    float spawnX;
    public GameObject deathEffect;
    public event Action deathEvent;
    public event Action enemyPassedEvent;

    Animator animator;
    float animatorSpeedPercent;
    float animatorIdleNumber;

    protected override void Start()
    {
        base.Start();
        health = startingHealth;
        
        
        //initialMovementSpeed = 7f;
        movementSpeed = initialMovementSpeed;
        SetSpeed(movementSpeed);

        spawnX = transform.position.x;

        deathEvent += FindObjectOfType<EnemySpawner>().jumperSpawner.OnEnemyDeath;
        deathEvent += FindObjectOfType<CoinSpawner>().ToSpawnCoin;
        enemyPassedEvent += FindObjectOfType<EnemySpawner>().jumperSpawner.OnEnemyPassed;
        enemyPassedEvent += FindObjectOfType<PlayerMovement>().OnGeneralEnemyPassed;
        animator = GetComponent<Animator>();

        enemyType = "Jumper";
        
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
        base.OutOfBoundsDestroy();
        if(transform.position.z < 1f){
            if(enemyPassedEvent != null){
                enemyPassedEvent();
            }
        }
        
    }


    protected override void movement(){
        if(!whileJump){
            //Moves the enemy forward
            transform.Translate((-1*transform.forward) * speed * Time.deltaTime );


            //following finds the y value of the enemy while moving, so that it stays on top of the terrain
            float newY = -1000f;
            Vector3 rayOrigin = new Vector3(transform.position.x , 50f, transform.position.z);
            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, terrainMask)){
                newY = hit.point.y;
            }


            //Adjusts the vertical orientation of the enemy according to the average normal of the terrain 
            Vector3 terrainNormal = CalculateAverageNormalVector(transform.position.x, transform.position.z);
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);

            //uses the Y-value found before and terrain normal to set the y coord of the enemy 
            transform.position = transform.position + Vector3.up*(newY - transform.position.y + terrainNormal.normalized.y) ; 
        }
        else{
            //Debug.Log(transform.position.y);
            transform.Translate((-1*transform.forward.z * Vector3.forward) * speed * Time.deltaTime );

            //following finds the y value of the enemy while moving, so that it stays on top of the terrain
            float newY = -1000f;
            Vector3 rayOrigin = new Vector3(transform.position.x , 50f, transform.position.z);
            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, terrainMask)){
                newY = hit.point.y;
            }

            //if y-value of enemy is less than the y-value of the terrain, we will stop jumping || Mathf.Abs(transform.position.y - newY) < 0.5f
            Vector3 terrainNormal = CalculateAverageNormalVector(transform.position.x, transform.position.z);
            if(transform.position.y -1f < newY){
                //Debug.Log("Terrain y: "+newY);
                transform.position = transform.position + Vector3.up * (newY - transform.position.y + terrainNormal.normalized.y);

                //Start coroutine here to realignX here
                StartCoroutine(RealignX());

                whileJump = false;
            }

        }
        
    }

    IEnumerator RealignX(){
        while(transform.position.x != spawnX){
            transform.position = transform.position + Vector3.right * (spawnX - transform.position.x) * Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    float jumpCounter = 0f;
    void FixedUpdate()
    {
        if(!exploded){
            SetSpeed(movementSpeed);
            movement();
            OutOfBoundsDestroy();
            
            healthBar.fillAmount = health/startingHealth;
            
            RaycastHit hit;

            
            
            if(DetectWallAhead(5f, out hit)){
                //Jump();
                jumpCounter = 30f;
                Debug.Log("Jump");
                
                animator.SetBool("IsJumping", true);
                //SetSpeed(5f);
                
            }else{
                animator.SetBool("IsJumping", false);
                //SetSpeed(7f);
            }

            if(jumpCounter != 0f){
                jumpCounter --;
            }

            if(jumpCounter == 0){
                animatorSpeedPercent = 0.5f;
            } else {
                animatorSpeedPercent = 1f;
            }

            
            
            //animator.SetFloat("state", animatorSpeedPercent);

            transform.position -= Vector3.up;
        }
    }

}
