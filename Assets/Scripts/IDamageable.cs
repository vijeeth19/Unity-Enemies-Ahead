﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable{
    void TakeHit(float damage, Vector3 hitPoint);
    float GetHealth();
}