using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SprinterEnemy : Enemy
{
    public float startingHealth = 5;
    //public Image healthBar;
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

        //initialMovementSpeed = 15f;
        movementSpeed = initialMovementSpeed;
        SetSpeed(movementSpeed);

        deathEvent += FindObjectOfType<EnemySpawner>().sprinterSpawner.OnEnemyDeath;
        deathEvent += FindObjectOfType<CoinSpawner>().ToSpawnCoin;
        enemyPassedEvent += FindObjectOfType<PlayerMovement>().OnGeneralEnemyPassed;
        enemyPassedEvent += FindObjectOfType<EnemySpawner>().sprinterSpawner.OnEnemyPassed;

        animator = GetComponent<Animator>();
        //transform.rotation = Quaternion.Euler(0, 180f, 0);
        animatorIdleNumber = (float) UnityEngine.Random.Range(0, 3);
        enemyType = "Sprinter";
        
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

    // Update is called once per frame
    void Update()
    {
        if(!exploded){
            movement();
            OutOfBoundsDestroy();

            healthBar.fillAmount = health/startingHealth;
            
            RaycastHit hit;
            if(DetectWallAhead(3f, out hit)){
                SetSpeed(0f);
                //animatorSpeedPercent = animatorIdleNumber;
                animatorSpeedPercent = Mathf.Lerp(animator.GetFloat("speedPercent"), animatorIdleNumber, 2f * Time.deltaTime);
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
