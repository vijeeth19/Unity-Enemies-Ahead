using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFreezable
{
    void TakeIce(float damage, float speedDecrease, float time, Vector3 hitPoint, GameObject effect);
}
