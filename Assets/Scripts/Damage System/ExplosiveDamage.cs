using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExplosiveDamage : ADamager
{
    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                    //float explosionRadius, float explosionPower, float upwardForce, float explosionDirectDamge
           
        Collider[] colliders = Physics.OverlapSphere(contactPoint, AttackArg1);
        Debug.Log("Number of colliders " + colliders.Length);
        try{
            foreach (Collider hit in colliders)
            {
                IExplodable explodableObject = hit.GetComponent<IExplodable>();
                Debug.Log("Tag: " + hit.tag);
                if(explodableObject != null && !explodableObject.HasExploded()){
                    Debug.Log("explodable collider");
                    
                    explodableObject.TakeExplosion(AttackArg2, contactPoint, AttackArg1, AttackArg3, AttackArg4);
                }
                else if(string.Compare(hit.tag,"Enemy") == 0){ 
                    IExplodable explodableObject2 = hit.transform.parent.parent.parent.parent.GetComponent<IExplodable>();
                    if(explodableObject2 != null && !explodableObject2.HasExploded()){
                        explodableObject2.TakeExplosion(AttackArg2, contactPoint, AttackArg1, AttackArg3, AttackArg4);
                    }
                }
            }
        }catch(NullReferenceException e){}
        
        Destroy(Instantiate(effect.gameObject, source.transform.position, Quaternion.identity) as GameObject, 2f);
        //Destroy(source);
        source.SetActive(false);


    }
}
