using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElectricDamage : ADamager
{
    public override void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2){
                                                                            //float damage, float mode, float haltTime, float radius
        
        if((int)AttackArg2 == 0){
            IShockable shockableObject = collider.GetComponent<IShockable>();

            try{
                if(shockableObject != null){
                    shockableObject.TakeShock(AttackArg1, 0f, AttackArg3, contactPoint, effect);
                }
                else if((string.Compare(collider.tag,"Enemy") == 0)){
                    IShockable shockableObject2 = collider.transform.parent.parent.parent.parent.GetComponent<IShockable>();
                    if(shockableObject2 != null){
                        shockableObject2.TakeShock(AttackArg1, 0f, AttackArg3, contactPoint, effect);
                    }
                }
            }
            catch(NullReferenceException e){}
        }
        else{

            Collider[] colliders = Physics.OverlapSphere(contactPoint, AttackArg4);

            try{
                foreach (Collider hit in colliders)
                {
                    IShockable shockableObject = hit.GetComponent<IShockable>();

                    if(shockableObject != null){
                        shockableObject.TakeShock(AttackArg1, AttackArg2, AttackArg3, contactPoint, effect);
                    }
                    else if(string.Compare(hit.tag,"Enemy") == 0){ 
                        IShockable shockableObject2 = hit.transform.parent.parent.parent.parent.GetComponent<IShockable>();
                        if(shockableObject2 != null){
                            shockableObject2.TakeShock(AttackArg1, AttackArg2, AttackArg3, contactPoint, effect);
                        }
                    }
                }
            }catch(NullReferenceException e){}

        }
        
        Destroy(Instantiate(effect2.gameObject, source.transform.position + Vector3.up*20f, Quaternion.identity) as GameObject, 0.25f);
        //Destroy(source);
        source.SetActive(false);

    }
}
