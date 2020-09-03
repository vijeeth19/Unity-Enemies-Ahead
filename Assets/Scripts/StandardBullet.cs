using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StandardBullet : MonoBehaviour
{
    
    public ADamager damager;
    public float AttackArg1;
    public float AttackArg2;
    public float AttackArg3;
    public float AttackArg4;
    public GameObject attackEffect1;
    public GameObject attackEffect2;
    Vector3 lastPosition;
    public Rigidbody rb;
    public bool destroyDelay = false;

    void OnEnable(){
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    protected virtual void OnCollisionEnter(Collision collision){

        damager.Attack(gameObject, collision.GetContact(0).point, AttackArg1, AttackArg2, AttackArg3, AttackArg4, collision.collider, attackEffect1, attackEffect2);
        if(destroyDelay) StartCoroutine(DestroyBullet());
    }
    IEnumerator DestroyBullet(){
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }

    void Start(){
        lastPosition = transform.position;
    }
    void FixedUpdate(){
        Vector3 deltaPos = transform.position - lastPosition;
        Quaternion lookRotation = Quaternion.LookRotation(deltaPos);
        transform.rotation = lookRotation;
        lastPosition = transform.position;

        if(transform.position.y < -5f) gameObject.SetActive(false);
    }

}
