using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BasicEnemy : Enemy
{
    public float startingHealth = 10;
    //public Image healthBar;
    public GameObject deathEffect;
    public event Action deathEvent;
    public event Action enemyPassedEvent;

    Animator animator;
    float animatorSpeedPercent;
    float animatorIdleNumber;
    bool started = false;

    protected override void Start()
    {
        base.Start();
        health = startingHealth;
        started = true;
        //initialMovementSpeed = 5f;
        movementSpeed = initialMovementSpeed;
        SetSpeed(movementSpeed);
        
        deathEvent += FindObjectOfType<EnemySpawner>().basicSpawner.OnEnemyDeath;
        deathEvent += FindObjectOfType<CoinSpawner>().ToSpawnCoin;
        enemyPassedEvent += FindObjectOfType<EnemySpawner>().basicSpawner.OnEnemyPassed;
        enemyPassedEvent += FindObjectOfType<PlayerMovement>().OnGeneralEnemyPassed;

        animator = GetComponent<Animator>();
        //transform.rotation = Quaternion.Euler(0, 180f, 0);
        animatorIdleNumber = (float) UnityEngine.Random.Range(0, 3);
        enemyType = "Basic";
        
    }
    

    public override void TakeHit(float damage, Vector3 hitPoint){
        if (damage >= health & !dead) {
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

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Euler(0, 180f, 0);
        
        if(!exploded){
            movement();
            //transform.position -= Vector3.up;
            OutOfBoundsDestroy();

            healthBar.fillAmount = health/startingHealth;
            
            RaycastHit hit;
            if(DetectWallAhead(2f, out hit)){
                SetSpeed(0f);
                animatorSpeedPercent = animatorIdleNumber;
            }
            else{
                SetSpeed(movementSpeed);
                animatorSpeedPercent = 3;
            }

            animator.SetFloat("speedPercent", animatorSpeedPercent);

            transform.position -= Vector3.up;
        }
        
        
    }
}
