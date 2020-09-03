using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Turret : Building
{
    public Transform target;
    float turretRange = 15f;
    public float turretFireRate = 1f;
    private float fireCountDown = 0f;

    public GameObject turretMuzzleHolder;
    public GameObject finalBulletPrefab;
    public GameObject bulletPrefab;
    public GameObject level2BulletPrefab;
    public GameObject level3BulletPrefab;

    public Transform firePoint;

    public int level;

    public bool shootable;

    public Material opaqueMat;
    public Material transparentMat;
    public Material muzzleHolderMat;
    Renderer[] children;

    public float turretCenterX, turretCenterZ;

    public MeshRenderer level1Sides, level2Sides, level3Sides;


    void Start()
    {
        base.Start();

        InvokeRepeating("FindClosestTarget", 0f, 1.5f);
        instanceBuildStatus = buildStatus.Building;

        GetComponent<CapsuleCollider>().enabled = false;

        children = GetComponentsInChildren<MeshRenderer>();

        Debug.Log("children length: " + children.Length);

        shootable = false;
        
        SetStartingLife(200f);
        SetDeathRate(1f);
        SetCost(25);

        finalBulletPrefab = bulletPrefab;
        if(level1Sides != null) UpdateLevelSettings();
        
    }

    public void UpdateLevelSettings(){
        if(level == 1) { level1Sides.enabled = true; finalBulletPrefab = bulletPrefab;}
        if(level == 2) { level2Sides.enabled = true; finalBulletPrefab = level2BulletPrefab;}
        if(level == 3) { level3Sides.enabled = true; finalBulletPrefab = level3BulletPrefab;}
    }

    void FindClosestTarget(){
        GameObject[] enemiesActive = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemiesActive.Length == 0) target = null;
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach(GameObject enemy in enemiesActive){
            if(enemy.GetComponent<Enemy>() != null){
                if(enemy.GetComponent<Enemy>().HasExploded()) continue;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance){
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }

            if(nearestEnemy != null && shortestDistance <= turretRange){
                if(nearestEnemy.GetComponent<Enemy>() != null){
                    if(!nearestEnemy.GetComponent<Enemy>().HasExploded()){
                        target = nearestEnemy.transform;
                    }
                    else{
                        target = null;
                    }
                }
                
            }
        }
    }




    // Update is called once per frame
    void Update()
    {
        base.Update();

        if(instanceBuildStatus == buildStatus.Built){
            shootable = true;
            GetComponent<CapsuleCollider>().enabled = true;
        } 

        if(target == null){
            return;
        }
        else{
            if(target.GetComponent<Enemy>() != null){
                if(target.GetComponent<Enemy>().HasExploded()) return;
            }
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        Vector3 rotation = Quaternion.Lerp(turretMuzzleHolder.transform.rotation, lookRotation, 10f * Time.deltaTime).eulerAngles;
        
        if(shootable) turretMuzzleHolder.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if(fireCountDown <= 0f){
            if(shootable){
                Shoot();
                fireCountDown = 1/turretFireRate;
            }
            
        }

        fireCountDown -= Time.deltaTime;

        
    }

    void Shoot(){
        GameObject bulletInstance = (GameObject) Instantiate(finalBulletPrefab, firePoint.position, Quaternion.Euler(firePoint.rotation.eulerAngles.x, firePoint.rotation.eulerAngles.y + 90f, firePoint.rotation.eulerAngles.z));
        TurretBullet bullet = bulletInstance.GetComponent<TurretBullet>();

        if(bullet != null){
            bullet.Seek(target);
        }
    }
}
