using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBurnable
{
    void TakeFire(float damage, float timeInterval, int iterations, Vector3 hitPoint, GameObject effect);
}
