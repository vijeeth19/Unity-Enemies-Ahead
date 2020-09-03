using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryScript : MonoBehaviour
{
    public int lengthOfLineRenderer = 40;
    public LineRenderer line ;
    public float initVelocityY = 10f;
    public float initVelocityX;
    float initHeight;
    public Vector3 endPoint;

    public Transform player;

    public float tangentialDistance;
    public Vector3 tangentialDirection;
    float timeToFall;
    float timeInterval;


    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = lengthOfLineRenderer;
        line.enabled = false;
        initHeight = transform.position.y;
        //Debug.Log( initHeight );
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + new Vector3(1.5f,0,0);
        tangentialDistance = FindTangentialDistance(endPoint);
        
        tangentialDirection = FindTangentialDirection(endPoint);
        initHeight = FindHeightFromPoint(endPoint);

        //Physics.gravity.y is currently -9.81
        timeToFall = CalculateTimeToFall(initVelocityY, initHeight, Physics.gravity.y);
        //Debug.Log(timeToFall + " " + initVelocityY + " " + initHeight + " " + Physics.gravity.y );

        initVelocityX = tangentialDistance/timeToFall;
        timeInterval = timeToFall/(lengthOfLineRenderer-1);


        
        Vector3 previousPoint = new Vector3(0f,0f,0f);
        
        for(int i = 0; i < lengthOfLineRenderer; i++ ){
            float time = timeInterval*i;
            float Sv = initVelocityY *time + Physics.gravity.y/2 * time *time;
            float Sh = initVelocityX *time;

            Vector3 normalSv = Vector3.up * Sv;
            Vector3 tangentialSh = tangentialDirection * Sh;

            Vector3 nextPoint = previousPoint + normalSv + tangentialSh;

            line.SetPosition(i, nextPoint);

        }
        
        //Debug.Log(line.GetPosition(39));
    }

    float CalculateTimeToFall(float upwardVelocity, float initHeight, float gravity){
        float t = 0;
        float d = upwardVelocity*t + gravity/2*t*t + initHeight;

        while(d>0.00001f){
            t += 0.00001f;
            d = upwardVelocity*t + gravity/2*t*t + initHeight;
        }
        return t;
    }

    float FindTangentialDistance(Vector3 endPoint){
        Vector3 directionalVector = endPoint - transform.position;
        Vector3 tangentialVector = directionalVector - directionalVector.y*Vector3.up;
        float tangentialDist = tangentialVector.magnitude;

        return tangentialDist;
    }

    float FindHeightFromPoint(Vector3 endPoint){
        Vector3 directionalVector = endPoint - transform.position;
        
        float height = -1 * directionalVector.y;
        return height;
    }

    Vector3 FindTangentialDirection(Vector3 endPoint){
        Vector3 directionalVector = endPoint - transform.position;
        Vector3 tangentialVector = directionalVector - directionalVector.y*Vector3.up;
        Vector3 tangentialDir = tangentialVector.normalized;

        return tangentialDir;
    }

}
