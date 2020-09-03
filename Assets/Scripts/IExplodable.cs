using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplodable{
    void TakeExplosion(float power, Vector3 explosionPoint, float radius, float upwardForce, float directHitDamage);
    bool HasExploded();
}
