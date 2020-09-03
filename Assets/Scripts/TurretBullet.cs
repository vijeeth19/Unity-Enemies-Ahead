using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TurretBullet : MonoBehaviour
{
    private Transform target;
    float speed = 20f;
    
    public ADamager damager;
    public float AttackArg1;
    public float AttackArg2;
    public float AttackArg3;
    public float AttackArg4;
    public GameObject attackEffect1;
    public GameObject attackEffect2;
    Vector3 lastPosition;

    void Start()
    {
        
    }

    public void Seek(Transform _target){
        target = _target;
    }

    public void HitTarget(){
        Debug.Log("We hit something");
    }

    void OnCollisionEnter(Collision collision){
    
        /*
        if(string.Compare(collision.collider.tag,"Wall") != 0){
            IDamageable damageableObject = collision.collider.GetComponent<IDamageable>();
            try{
                if(damageableObject != null){
                    damageableObject.TakeHit(2f, collision.GetContact(0).point);
                    //Debug.Log("I hit something damageable!");
                }
                else if(collision.collider.transform.parent.parent.parent.parent.GetComponent<IDamageable>() != null){
                    collision.collider.transform.parent.parent.parent.parent.GetComponent<IDamageable>().TakeHit(2f, collision.GetContact(0).point);
                }
            }
            catch(NullReferenceException e){

            }

            DestroyBullet();
        }*/
        
        damager.Attack(gameObject, collision.GetContact(0).point, AttackArg1, AttackArg2, AttackArg3, AttackArg4, collision.collider, attackEffect1, attackEffect2);
        DestroyBullet();

    }

    void DestroyBullet(){
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null){
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position + Vector3.up )- transform.position;
        if(direction.normalized.y > 0.4f)
        direction = direction - Vector3.up*direction.y;
        float distanceThisFrame = speed * Time.deltaTime;

        if(direction.magnitude <= distanceThisFrame){
            //HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

        
    }
}
