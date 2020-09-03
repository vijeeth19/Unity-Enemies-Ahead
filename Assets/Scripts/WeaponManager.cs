using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public float turretStartPrice = 20f;
    public float brickStartPrice = 5f;
    public float turretPrice;
    public float brickPrice;
    CoinManager coinManager;

    void Start()
    {
        turretPrice = turretStartPrice;
        brickPrice = brickStartPrice;
        coinManager = FindObjectOfType<CoinManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
