using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ADamager: MonoBehaviour
{
    public abstract void Attack(GameObject source, Vector3 contactPoint, float AttackArg1, float AttackArg2, float AttackArg3, float AttackArg4, Collider collider, GameObject effect, GameObject effect2);
}
