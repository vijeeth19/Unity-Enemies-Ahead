using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExplosiveBullet : StandardBullet
{
    public GameObject explosionEffect;
    public ParticleSystem smokeTrail;
    Vector3 lastPosition;

    void Start(){
        //Instantiate(smokeTrail.gameObject, transform.position, Quaternion.identity) as GameObject;
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision){
        Vector3 explosionPoint;
        if(string.Compare(collision.collider.tag,"Wall") != 0){
            IDamageable damageableObject = collision.collider.GetComponent<IDamageable>();

            explosionPoint = collision.GetContact(0).point;
            
            Collider[] colliders = Physics.OverlapSphere(explosionPoint, 5f);
            Debug.Log("Number of colliders " + colliders.Length);
            try{
                foreach (Collider hit in colliders)
                {
                    IExplodable explodableObject = hit.GetComponent<IExplodable>();
                    Debug.Log("Tag: " + hit.tag);
                    if(explodableObject != null && !explodableObject.HasExploded()){
                        Debug.Log("explodable collider");
                        
                        explodableObject.TakeExplosion(400f, explosionPoint, 8f, 10f, 10f);
                    }
                    else if(string.Compare(hit.tag,"Enemy") == 0){ 
                        IExplodable explodableObject2 = hit.transform.parent.parent.parent.parent.GetComponent<IExplodable>();
                        if(explodableObject2 != null && !explodableObject2.HasExploded()){
                            explodableObject2.TakeExplosion(400f, explosionPoint, 8f, 10f, 10f);
                        }
                    }
                    
                }
            }catch(NullReferenceException e){}
            
            Destroy(Instantiate(explosionEffect.gameObject, transform.position, Quaternion.identity) as GameObject, 2f);
            Destroy(gameObject);
        }        

    }

    void FixedUpdate(){
        Vector3 deltaPos = transform.position - lastPosition;
        Quaternion lookRotation = Quaternion.LookRotation(deltaPos);
        //var newShape = smokeTrail.shape;
        //newShape.rotation = 
        //transform.rotation = Quaternion.FromToRotation(transform.forward, deltaPos);
        transform.rotation = lookRotation;
        Debug.Log(transform.rotation.eulerAngles + " - " + transform.forward);
        lastPosition = transform.position;
    }
}
