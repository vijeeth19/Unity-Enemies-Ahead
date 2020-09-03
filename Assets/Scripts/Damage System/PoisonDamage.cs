using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoisonDamage : ADamager
{
    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                        //float Damage, float timeInterval, float damageDecreasePercent, float radius

        Collider[] colliders = Physics.OverlapSphere(contactPoint, AttackArg4);

            try{
                
                foreach (Collider hit in colliders)
                {
                    IPoisonable poisonableObject = hit.GetComponent<IPoisonable>();

                    if(poisonableObject != null){
                        poisonableObject.TakePoison(AttackArg1, AttackArg2,AttackArg3, contactPoint, effect);
                    }
                    else if(string.Compare(hit.tag,"Enemy") == 0){ 
                        IPoisonable poisonableObject2 = hit.transform.parent.parent.parent.parent.GetComponent<IPoisonable>();
                        if(poisonableObject2 != null){
                            poisonableObject2.TakePoison(AttackArg1, AttackArg2,AttackArg3, contactPoint, effect);
                        }
                    }
                }
            }catch(NullReferenceException e){}

        
        //Destroy(Instantiate(effect.gameObject, source.transform.position + 0.2f*Vector3.up, Quaternion.identity) as GameObject, 3f);
        
        //Destroy(source);
        source.SetActive(false);
    }
}