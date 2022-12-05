using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointVariables : MonoBehaviour
{
    public float Radius1 = 2f;
    public float Radius2 = 2f;


    public float rotationLerpPower = 0.05f;

    public void SetLerp(PointVariables p1, PointVariables p2, float lerp)
    {
        Radius1 = Mathf.Lerp(p1.Radius1, p2.Radius1, lerp);
        Radius2 = Mathf.Lerp(p1.Radius2, p2.Radius2, lerp);
        
        rotationLerpPower = Mathf.Lerp(p1.rotationLerpPower, p2.rotationLerpPower, lerp);
        rotationLerpPower = Mathf.Lerp(p1.rotationLerpPower, p2.rotationLerpPower, lerp);
    }
}
