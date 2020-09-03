using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour, IDamageable, IExplodable, IBurnable, IShockable, IFreezable, IPoisonable
{
    public float initialMovementSpeed;
    protected float movementSpeed, speed;
    public bool dead;
    protected float health;
    public float attackDamage;
    public LayerMask wallMask;
    public LayerMask terrainMask;
    public bool exploded = false;
    public bool frozen = false;
    protected String enemyType;
    public delegate void EnemyExplodedEventHandler(object source, EnemyDeathEventArgs args);
    public event EnemyExplodedEventHandler EnemyExploded;
    public Image healthBar;
    SkinnedMeshRenderer renderer;
    Material[] originalMats;
    Animator anim;
    protected int myLayer;
    public event EventHandler<float> EnemyPassedDamage;
    private Manager manager;
    

    protected virtual void Start()
    {
        //Make sure the enemy is facing completely forward
        transform.forward = transform.forward - Vector3.right*transform.forward.x;
        EnemyExploded += FindObjectOfType<EnemySpawner>().OnEnemyExploded;
        EnemyExploded += FindObjectOfType<CoinSpawner>().OnEnemyExploded;
        EnemyPassedDamage += FindObjectOfType<PlayerMovement>().OnEnemyAttacked;

        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        
        originalMats = new Material[renderer.materials.Length];
        for(int i = 0; i < originalMats.Length; i++){
            originalMats[i] = renderer.materials[i];
        }
        
        anim = GetComponent<Animator>();
        myLayer = gameObject.layer;
        
        manager = FindObjectOfType<Manager>();

    }

    public float GetHealth(){
        return health;
    }

    public void Destroy(){
        
        for(int i = 0; i < originalMats.Length; i++){
            Destroy(renderer.materials[i]);
        }
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    
    public virtual void OutOfBoundsDestroy(){
        if(transform.position.z < 1f){
            Destroy();
            EnemyPassedDamage?.Invoke(this, attackDamage);
        }
    }
    
    public virtual void TakeHit(float damage, Vector3 hitPoint){
        health -= damage;
        
        if(health <= 0 && !dead){
            Die();
        }
        else if(damage > 0.5f){
            StartCoroutine("DamageFlash");
        }   
    }

    public virtual void Die(){
        dead = true;
        FindObjectOfType<CoinSpawner>().deathPos = transform.position;
        Destroy();
        manager.IncrementEnemiesKilled(1);
    }
    
    IEnumerator DamageFlash(){
        
        for(int i = 0; i < originalMats.Length; i++){
            originalMats[i].color = originalMats[i].color + new Color(1f,0.1f,0.1f,0.3f);
        }
        renderer.materials = originalMats;
        
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i < originalMats.Length; i++){
            originalMats[i].color = originalMats[i].color - new Color(1f,0.1f,0.1f,0.3f);;
        }
        renderer.materials = originalMats;
        
        
    }

    public String GetEnemyType(){
        return enemyType;
    }

    public bool HasExploded(){
        return exploded;
    }

    public void TakeExplosion(float power, Vector3 explosionPoint, float radius, float upwardForce, float directHitDamage){
        /*
        Vector3 endPos = Vector3.zero;
        switch(enemyType){
            case "Basic":
                endPos = transform.position + Vector3.up*1.15f; break;
            case "Sprinter":
                endPos = transform.position + Vector3.up*2f; break;
            case "Jumper":
                endPos = transform.position + Vector3.up*2f; break;
            case "Dodger":
                endPos = transform.position + Vector3.up*2.3f; break;
            case "Tank":
                endPos = transform.position + Vector3.up*3.2f; break;
            default:
                break;
        }
        float hitDamage = Mathf.Max(0f, directHitDamage * ( 1f - ((explosionPoint - endPos).magnitude)/radius ));*/
        if(health - directHitDamage > 0f) TakeHit(directHitDamage, transform.position);
        else health -= directHitDamage;
        if(health <= 0){
            if(anim != null){
                anim.enabled = false;
            }
            healthBar.enabled = false;
            FindObjectOfType<CoinSpawner>().deathPos = transform.position; //spawn coin
            EnemyDeathEventArgs ed = new EnemyDeathEventArgs(enemyType); //explosion event
            OnEnemyExploded(ed); //call the event
            exploded = true;
            dead = true;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
            rb.AddExplosionForce(power, explosionPoint, radius, upwardForce);
            StartCoroutine("DieOfExplosion");
        }
    }
    IEnumerator DieOfExplosion(){
        gameObject.layer = 0;
        gameObject.tag = "Untagged";
        yield return new WaitForSeconds(3f);
        Destroy();
    }
    protected virtual void OnEnemyExploded(EnemyDeathEventArgs enemyDeath){
        if(EnemyExploded != null)
            EnemyExploded(this, enemyDeath);
    }

    public void TakeFire(float damage, float timeInterval, int iterations, Vector3 hitPoint, GameObject effect){
        GameObject FireEffectInstantiation = Instantiate(effect, hitPoint+0.3f*Vector3.up, Quaternion.identity);
        FireEffectInstantiation.transform.parent = this.transform;
        StartCoroutine(FireDamage(FireEffectInstantiation, damage, timeInterval, iterations, hitPoint));   
    }
    IEnumerator FireDamage(GameObject fei, float damage, float timeInterval, int iterations, Vector3 hitPoint){
        for(int i = 0; i < iterations; i++){
            yield return new WaitForSeconds(0.3f);
            TakeHit(damage, transform.position);
            yield return new WaitForSeconds(timeInterval);
        }
        Destroy(fei);
    }

    float preShockSpeed; int shockCoroutineCount = 0;
    public void TakeShock(float damage, float radius, float haltTime, Vector3 hitPoint, GameObject effect){
        StartCoroutine(ShockDamage(effect, damage, radius, haltTime, hitPoint));
    }
    IEnumerator ShockDamage(GameObject effect, float damage, float radius, float haltTime, Vector3 hitPoint){
        shockCoroutineCount++;
        if(shockCoroutineCount == 1) preShockSpeed = movementSpeed;
        

        yield return new WaitForSeconds(0.1f);
        GameObject ShockEffectInstantiation = Instantiate(effect, transform.position+1.2f*Vector3.up+0.2f*Vector3.back, Quaternion.identity);
        ShockEffectInstantiation.transform.parent = this.transform;
        yield return new WaitForSeconds(0.1f);
        TakeHit(damage, transform.position);
        
        
        movementSpeed = 0f;
        if(anim != null) anim.enabled = false;

        yield return new WaitForSeconds(haltTime);

        if(shockCoroutineCount == 1){
            movementSpeed = preShockSpeed;
            if(anim != null) anim.enabled = true;
        }
        
        
        Destroy(ShockEffectInstantiation);
        shockCoroutineCount--;
    }

    float preFreezeSpeed; int freezeCoroutineCount = 0;
    public void TakeIce(float damage, float speedDecreasePercent, float time, Vector3 hitPoint, GameObject effect){
        StartCoroutine(FreezeDamage(effect, damage, speedDecreasePercent, time, hitPoint));
    }
    IEnumerator FreezeDamage(GameObject effect, float damage, float speedDecreasePercent, float time, Vector3 hitPoint){
        freezeCoroutineCount++;
        if(freezeCoroutineCount == 1) preFreezeSpeed = movementSpeed;
        //Destroy(Instantiate(effect.gameObject, hitPoint + 0.2f*Vector3.up, Quaternion.identity) as GameObject, 3f);
        //yield return new WaitForSeconds(0.1f);
        TakeHit(damage, transform.position);
        movementSpeed = preFreezeSpeed*speedDecreasePercent;
        Debug.Log("SDP " + speedDecreasePercent);
        if(anim != null) anim.speed = speedDecreasePercent;
        for(int i = 0; i < originalMats.Length; i++){
            //originalMats[i].color = Color.Lerp(originalMats[i].color, new Color(0.1f,1f,1f,1f), 0.3f);
            originalMats[i].color = originalMats[i].color + new Color(0.1f,1f,1f,0.3f)*0.5f;
        }
        renderer.materials = originalMats;

        yield return new WaitForSeconds(time);
        
        for(int i = 0; i < originalMats.Length; i++){
            originalMats[i].color = originalMats[i].color - new Color(0.1f,1f,1f,0.3f)*0.5f;
        }
        renderer.materials = originalMats;

        if(freezeCoroutineCount == 1){
            movementSpeed = preFreezeSpeed;
            if(anim != null) anim.speed = 1f;
        }
        freezeCoroutineCount--;
    }

    float poisonCoroutineCount = 0;
    public void TakePoison(float damage, float timeInterval, float damageDecreasePercent, Vector3 hitPoint, GameObject effect){
        GameObject poisonEffectInstantiation = Instantiate(effect, transform.position+0.7f*Vector3.up, Quaternion.identity);
        poisonEffectInstantiation.transform.parent = this.transform;
        StartCoroutine(PoisonDamage(poisonEffectInstantiation, damage, timeInterval, damageDecreasePercent, hitPoint));
    }
    IEnumerator PoisonDamage(GameObject pei, float damage, float timeInterval, float damageDecreasePercent, Vector3 hitPoint){
        poisonCoroutineCount++;
        if(poisonCoroutineCount == 1) attackDamage *= damageDecreasePercent;
        yield return new WaitForSeconds(0.3f);
        for(int i = 0; i < 3; i++){
            TakeHit(damage, transform.position);
            yield return new WaitForSeconds(timeInterval);
        }
        Destroy(pei);
        poisonCoroutineCount--;
    }

    protected void SetSpeed(float _speed){
        speed = _speed;
        if(_speed == 0f) gameObject.layer = myLayer;
        else gameObject.layer = 0;
    }

    protected Vector3 CalculateAverageNormalVector(float terrainX, float terrainZ){
        int samples = 10;
        Vector3 [,] normalVectorData = new Vector3[samples,samples];
        float testingX;
        float testingZ;
        RaycastHit hit;
        float sumX = 0f;
        float sumY = 0f;
        float sumZ = 0f;

        for(int i = 0; i < samples; i++ ){
            for(int j = 0; j < samples; j++ ){
                float spaceInterval = 0.2f;

                testingX = terrainX - 1 + j * spaceInterval;
                testingZ = terrainZ - 1 + i * spaceInterval;

                
                Vector3 rayOrigin = new Vector3(testingX, 20f, testingZ);
                Ray ray = new Ray(rayOrigin, Vector3.down);
                if(Physics.Raycast(ray, out hit, 100f, terrainMask)){
                    normalVectorData[i,j] = hit.normal.normalized;
                    sumX += hit.normal.normalized.x;
                    sumY += hit.normal.normalized.y;
                    sumZ += hit.normal.normalized.z;
                }

            }
        }

        Vector3 avgNormalVector = new Vector3(sumX/100f, sumY/100, sumZ/100);
        return avgNormalVector;

    }

    //[NOTE] Transform position can be directly set or assigned a Vector3 value using '='
    protected virtual void movement(){
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
        //transform.position = transform.position + Vector3.up*(newY - transform.position.y+1) ;
    }

    protected bool DetectWallAhead(float distance, out RaycastHit hit){

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

        Vector3 rayOrigin = transform.position - 0.75f*surfaceNormal;
        //Vector3 rayOrigin = transform.position + 0.25f*surfaceNormal;
        //RaycastHit hit;
        Ray ray = new Ray(rayOrigin, forwardVector.normalized);

        //Debug.Log(transform.position + " " + transform.position + forwardVector*5f);
        Debug.DrawLine(rayOrigin, rayOrigin + forwardVector.normalized*distance);

        if(Physics.Raycast(ray, out hit, distance, wallMask)){
            
            return true;
        }
        else{
            return false;
        }
    }

    
    

}
