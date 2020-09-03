using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    float movementSpeed = 10f;
    float translation;
    public float terrainHeight;
    public LayerMask mask;
    public float playerStartingHealth = 100f;
    public bool playerDead;
    public float playerHealth;

    public Image playerHealthBar;
    public Image bloodOverlay;
    public float bloodAlpha;
    public float healthBarHeight;
    public float healthBarCorrectWidthFillAmount;
    Vector2 healthBarSize;

    public Manager manager;
    
    void Start()
    {
        playerDead = false;
        playerHealth = playerStartingHealth;
        bloodAlpha = 0f;
        bloodOverlay.color = new Vector4(255f, 0f, 0f, bloodAlpha);
        healthBarHeight = 50;
        healthBarSize = playerHealthBar.rectTransform.sizeDelta;
    }

    public void OnEnemyAttacked(object sender, float attackDamage){
        if(!playerDead){
            playerHealth -= attackDamage;
            bloodAlpha = 0.6f + 0.4f/15f * (attackDamage - 5f);
            bloodOverlay.color = new Vector4(255f, 0f, 0f, bloodAlpha);
            StartCoroutine("GetHurt");

            playerHealthBar.color = new Vector4(255f, 0f, 0f, 1f);
            healthBarHeight = 100f;
            playerHealthBar.rectTransform.sizeDelta = healthBarSize + Vector2.up*healthBarSize.y*0.5f; //20% increase in the y coordinate
            StartCoroutine("HealthBarHeightAnimation");

            healthBarCorrectWidthFillAmount = playerHealth/playerStartingHealth;
            StartCoroutine("HealthBarWidthAnimation");
        }
    }

    public void OnGeneralEnemyPassed(){
        //Debug.Log("Someone hurt me!");
        //playerHealth -= 10f;

        //Debug.Log(playerHealthBar.rectTransform.sizeDelta);
        //health UI effects
        /*
        bloodAlpha = 0.6f;
        bloodOverlay.color = new Vector4(255f, 0f, 0f, bloodAlpha);
        StartCoroutine("GetHurt");

        playerHealthBar.color = new Vector4(255f, 0f, 0f, 1f);
        healthBarHeight = 100f;
        playerHealthBar.rectTransform.sizeDelta = healthBarSize + Vector2.up*healthBarSize.y*0.5f; //20% increase in the y coordinate
        StartCoroutine("HealthBarHeightAnimation");

        healthBarCorrectWidthFillAmount = playerHealth/playerStartingHealth;
        StartCoroutine("HealthBarWidthAnimation");*/
        
    }

    IEnumerator GetHurt(){
        while(bloodAlpha > 0){
            bloodAlpha -= 0.01f;
            bloodOverlay.color = new Vector4(255f, 0f, 0f, bloodAlpha);
            yield return null;
        }
    }

    IEnumerator HealthBarWidthAnimation(){
        while(playerHealthBar.fillAmount > healthBarCorrectWidthFillAmount){
            playerHealthBar.fillAmount -=0.01f;
            yield return null;
        }
    }

    IEnumerator HealthBarHeightAnimation(){
        while(playerHealthBar.rectTransform.sizeDelta.y > healthBarSize.y){
            float healthBarHeight = playerHealthBar.rectTransform.sizeDelta.y - 1f;
            playerHealthBar.rectTransform.sizeDelta = new Vector2(healthBarSize.x, healthBarHeight);
            yield return null;
        }
    }

    void Die(){
        if(playerHealth <= 0){
            if(!playerDead) manager.OnGameOver();
            playerDead = true;
            
        }
        
        
    }

    void movement(){
        translation = Input.GetAxis("Horizontal")*movementSpeed*Time.deltaTime;
        
        RaycastHit hit;
        Vector3 origin = new Vector3(0f,20f,1f);
        Vector3 endPoint = transform.position;
        Ray ray = new Ray(origin, (endPoint-origin).normalized);

        Physics.Raycast(ray, out hit, 100f, mask);
        //Debug.Log(hit.point.y);
        //transform.position += Vector3.up * (hit.point.y - transform.position.y + 1.1f);

        if(translation < 0){
            if(transform.position.x < -4){
                translation = 0f;
            }
        }
        if(translation > 0){
            if(transform.position.x > 4){
                translation = 0f;
            }
        }

        transform.Translate(translation,0,0);
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(playerHealthBar.rectTransform.sizeDelta);
        movement();
        //playerHealthBar.fillAmount = playerHealth/playerStartingHealth;
        Die();
        //bloodOverlay.color = new Vector4(255, 0, 0, 0);
        if(Mathf.Abs(playerHealthBar.fillAmount - healthBarCorrectWidthFillAmount) <= 0.01f){
            playerHealthBar.color = new Vector4(0, 0f, 255f, 0.8f);
        }
        
    }

}
