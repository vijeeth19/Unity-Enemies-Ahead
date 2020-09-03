using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CoinSpawner : MonoBehaviour
{
    public Coin coinPrefab;
    public Vector3 deathPos;
    

    void Start()
    {
        deathPos = Vector3.zero;
        //FindObjectOfType<Enemy>().EnemyExploded += this.OnEnemyExploded;
    }

    public void ToSpawnCoin(){
        Coin coinInstance;
        
        Debug.Log("Spawn Coin" + deathPos);
        
        coinInstance = Instantiate(coinPrefab, deathPos, Quaternion.Euler(-90,0,0));
        //almuns++;
        

    }

    public void OnEnemyExploded(object source, EnemyDeathEventArgs enemyDeath){
        ToSpawnCoin();
    }

    
    void Update()
    {
        
    }
}
