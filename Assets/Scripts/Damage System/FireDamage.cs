using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireDamage : ADamager
{
    //public GameObject fireEffectPrefab;
    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                        //float Damage, float timeInterval, float iteration

        IBurnable burnableObject = collider.GetComponent<IBurnable>();

        try{
            if(burnableObject != null){
                burnableObject.TakeFire(AttackArg1, AttackArg2, (int) AttackArg3, contactPoint, effect);
            }
            else if((string.Compare(collider.tag,"Enemy") == 0)){
                if(collider.transform.parent.parent.parent.parent.GetComponent<IBurnable>() != null){
                    collider.transform.parent.parent.parent.parent.GetComponent<IBurnable>().TakeFire(AttackArg1, AttackArg2, (int) AttackArg3, contactPoint, effect);
                }
            }
            else{
                Destroy(Instantiate(effect.gameObject, contactPoint+0.3f*Vector3.up, Quaternion.identity) as GameObject, 3f);
            }
        }
        catch(NullReferenceException e){}
        //Destroy(source);
        source.SetActive(false);
        
    }
}
