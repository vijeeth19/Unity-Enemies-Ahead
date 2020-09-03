using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearAlgebra : MonoBehaviour
{
    public static List<Vector3> FindNormalVectors(Vector3 givenVector){
        Vector3 auxilaryVector = Vector3.zero;
        Vector3 normalA, normalB;
        Vector3[] basis = {Vector3.right, Vector3.up, Vector3.forward};
        for(int i = 0; i < 3; i++ ){
            auxilaryVector = givenVector + basis[i];
            if(!AreParallel(givenVector, auxilaryVector)){
                break;
            }
        }

        normalA = Vector3.Cross(givenVector, auxilaryVector).normalized;
        normalB = Vector3.Cross(givenVector, normalA).normalized;

        List<Vector3> vectorList = new List<Vector3>();
        vectorList.Add(normalA);
        vectorList.Add(normalB);

        return vectorList;
    }

    static bool AreParallel(Vector3 vectorA, Vector3 vectorB){
        Vector3 dirA, dirB;
        dirA = vectorA.normalized;
        dirB = vectorB.normalized;
        if(dirA == dirB)
            return true;
        else
            return false;
    }

}
