using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoisonable
{
    void TakePoison(float damage, float timeInterval, float damageDecreasePercent, Vector3 hitPoint, GameObject effect);
}