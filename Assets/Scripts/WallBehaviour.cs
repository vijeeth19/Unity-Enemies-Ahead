using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = new Vector4 (0.38f, 0.25f, 0.145f, 1f);
        GetComponent<BoxCollider>().enabled = false;
    }


    // Update is called once per frame
    void Update()
    {

            
    }
}
