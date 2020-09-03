using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShockable
{
    void TakeShock(float damage, float radius, float haltTime, Vector3 hitPoint, GameObject effect);
}
