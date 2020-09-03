using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnimation : MonoBehaviour
{
    private Animator anim;
    AnimatorStateInfo currInfo;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        //FindObjectOfType<EnemySpawner>().BetweenWavesEvent += OnBetweenWaves;
    }

    public void OnBetweenWaves(){
        anim.enabled = true;
        if (null != anim)
        {
            //anim.Play("WaveNumber");
            anim.SetTrigger("startAnim");
            Debug.Log("Wave Animation");
        }
        //anim.enabled = false;
    }

    public void OnNextWaveTriggered(){
        anim.SetTrigger("newWave");
    }

    void Update()
    {
        
    }
}
