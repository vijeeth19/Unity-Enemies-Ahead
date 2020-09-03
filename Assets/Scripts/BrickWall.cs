using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickWall : Building, IDamageable
{
    public float wallCenterX, wallCenterZ;

    
    Renderer[] children;
    public Material transparentMat, opaqueMat;


    
    void Start()
    {
        base.Start();

        GetComponent<Renderer>().material.color = new Vector4 (0.38f, 0.25f, 0.145f, 1f);
        GetComponent<BoxCollider>().enabled = false;

        children = GetComponentsInChildren<MeshRenderer>();

        SetStartingLife(40f);
        SetDeathRate(1f);
        SetCost(10);

    }

    public float GetHealth(){
        return -1f;
    }



    public void TakeHit(float damage, Vector3 hitPoint){
        if(instanceBuildStatus == buildStatus.Built){
            Destroy(gameObject);
            //UnblockNearblyWallPlacement();
        }

    }



    void Update()
    {
        base.Update();
        if(instanceBuildStatus == buildStatus.Built) GetComponent<BoxCollider>().enabled = true;
        
            
    }
}
