using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IceDamage : ADamager
{
    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                        //float Damage, float speedDecreasePercent, float time

        Collider[] colliders = Physics.OverlapSphere(contactPoint, AttackArg4);

            int hitNum = 0;
            try{
                
                foreach (Collider hit in colliders)
                {
                    IFreezable freezableObject = hit.GetComponent<IFreezable>();

                    if(freezableObject != null){
                        freezableObject.TakeIce(AttackArg1, AttackArg2, AttackArg3, contactPoint, effect);
                        hitNum++;
                    }
                    else if(string.Compare(hit.tag,"Enemy") == 0){ 
                        IFreezable freezableObject2 = hit.transform.parent.parent.parent.parent.GetComponent<IFreezable>();
                        if(freezableObject2 != null){
                            freezableObject2.TakeIce(AttackArg1, AttackArg2, AttackArg3, contactPoint, effect);
                        }
                        hitNum++;
                    }
                }
            }catch(NullReferenceException e){}

        
        Destroy(Instantiate(effect.gameObject, source.transform.position + 0.2f*Vector3.up, Quaternion.identity) as GameObject, 3f);
        if(hitNum > 0){
            if(effect2 != null) Destroy(Instantiate(effect2.gameObject, source.transform.position + 0.2f*Vector3.up, Quaternion.identity) as GameObject, 3f);
        } 
        //Destroy(source);
        source.SetActive(false);
    }
}