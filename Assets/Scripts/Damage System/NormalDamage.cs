using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalDamage: ADamager
{

    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                        //float damage
        if(string.Compare(collider.tag,"Wall") != 0){
            IDamageable damageableObject = collider.GetComponent<IDamageable>();

            try{
                if(damageableObject != null){
                    damageableObject.TakeHit(AttackArg1, contactPoint);
                    //Debug.Log("I hit something damageable!");
                }
                else if(collider.transform.parent.parent.parent.parent.GetComponent<IDamageable>() != null){
                    collider.transform.parent.parent.parent.parent.GetComponent<IDamageable>().TakeHit(5f, contactPoint);
                }else{
                }
            }
            catch(NullReferenceException e){}
            
            //Destroy(source, 0.3f);
        }

    }
    
}
