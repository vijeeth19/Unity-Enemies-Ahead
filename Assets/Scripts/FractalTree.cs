using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalTree : MonoBehaviour
{
    public GameObject branchPrefab;
    public int recursionLevel;
    private int highestLevel = 4;

    [Range(0.0f, 90.0f)]
    public float incrementAngle;

    // Start is called before the first frame update
    void Start()
    {
        MakeBranch(highestLevel, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), incrementAngle, 1f);
        //Debug.Log("Scale: " + branchPrefab.transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
        //MakeBranch(5, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), incrementAngle, 0.20f);
        
    }

    void MakeBranch(int level, Vector3 spawnPoint, Quaternion startingQuaternion, float incrementDegrees, float dampPercent){
        if(level == 0)
            return;
        
        GameObject branchInstance;

        branchInstance = Instantiate(branchPrefab, spawnPoint, startingQuaternion);
        branchInstance.transform.localScale -= Vector3.up*branchInstance.transform.localScale.y * (1f - dampPercent);
        float branchLength = branchInstance.transform.lossyScale.y;
        //Debug.Log("level: " + level + ", damp percent: " + dampPercent);
        branchInstance.transform.Translate(Vector3.up * branchLength, Space.Self);

        Vector3 branchSpawnPoint = spawnPoint + startingQuaternion * Vector3.up * branchLength * 2;
        //Debug.DrawLine(spawnPoint, spawnPoint + startingQuaternion * Vector3.up * 30f, Color.red, 50f);
        //Debug.Log(spawnPoint + " - " + (spawnPoint + startingQuaternion * Vector3.up) * 30f);


        float angle = Random.Range(0f, 2*Mathf.PI);
        Vector3 incrementAngleVector = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        float offsetAngle = Random.Range(0f, 2*Mathf.PI);

        for(int i = 0; i<3; i++){
            Quaternion rotationDivergingBranch = Quaternion.FromToRotation(Vector3.up, startingQuaternion * Vector3.up);
            //Debug.DrawLine(Vector3.zero, rotationDivergingBranch * Vector3.up * 10f, Color.green, 40f);
            //Vector3 incrementAngleVectorYRotated = (startingQuaternion * Vector3.up).normalized * (offsetAngle + (360f/3f)*i) + incrementAngleVector* incrementDegrees;
            Vector3 incrementAngleVectorYRotated = (startingQuaternion * Vector3.up).normalized * (offsetAngle + (360f/3f)*i);
            Quaternion incrementAngleVectorYRotatedQ = Quaternion.Euler(incrementAngleVectorYRotated);
            Vector3 bendBranchRotation = incrementAngleVector*incrementDegrees;
            Quaternion bendBranchRotationQ = Quaternion.Euler(incrementAngleVector*incrementDegrees);
            Debug.Log("Level " + level + " --- " + incrementAngleVectorYRotated);
            
            MakeBranch(level - 1, branchSpawnPoint, Quaternion.Euler(startingQuaternion.eulerAngles + bendBranchRotation + incrementAngleVectorYRotated), incrementDegrees+5f, dampPercent - (1f-0.35f)/((float) highestLevel));
        }
        
        /*
        float angle = Random.Range(0f, Mathf.PI);
        Vector3 incrementAngleVector = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        MakeBranch(level - 1, branchSpawnPoint, Quaternion.Euler(startingQuaternion.eulerAngles + incrementAngleVector * incrementDegrees), incrementDegrees+5f, dampPercent - (1f-0.35f)/((float) highestLevel));
        
        angle = Random.Range(0f, 2*Mathf.PI);
        incrementAngleVector = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        MakeBranch(level - 1, branchSpawnPoint, Quaternion.Euler(startingQuaternion.eulerAngles + incrementAngleVector * incrementDegrees), incrementDegrees+5f, dampPercent - (1f-0.35f)/((float) highestLevel));
        */

        //MakeBranch(level - 1, branchSpawnPoint, Quaternion.Euler(startingQuaternion.eulerAngles + Vector3.right * incrementDegrees), incrementDegrees, dampPercent * (dampPercent+1f));
        //MakeBranch(level - 1, branchSpawnPoint, Quaternion.Euler(startingQuaternion.eulerAngles + Vector3.forward * incrementDegrees), incrementDegrees, dampPercent * (dampPercent+1f));
        
        
    }
}
