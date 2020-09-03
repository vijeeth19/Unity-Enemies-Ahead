using UnityEngine;
using System.Collections;
 
public class Coin : MonoBehaviour {
 
    public GameObject meter;
    public Transform meterPosition;
    
    
    void Update () {
    
        transform.position = Vector3.Lerp(transform.position, meter.transform.position, 2f * Time.deltaTime);
        if((transform.position - meterPosition.position).magnitude < 1f){
            FindObjectOfType<CoinManager>().almuns++;
            Destroy(gameObject);
        }
        //Debug.Log();

    }
}
    