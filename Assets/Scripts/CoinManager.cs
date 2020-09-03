using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI almunsCount;
    public int almuns;

    void Start()
    {
        //PlayerPrefs.DeleteKey("FirstTime");
        almuns = 100;

    }

    public void DebitAlmuns(int amount){
        almuns -= amount;
    }

    void Update()
    {
        almunsCount.text = almuns.ToString();
    }
}
