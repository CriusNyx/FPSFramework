using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GyroSpring : SpringComponent
{
    public float dynamicStrength = 0f;
    public float staticStrength = 0f;

    protected override (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime)
    {
        Vector3 targetForward = rotation * Vector3.forward;
        Vector3 currentUp = this.rotation * Vector3.up;
        Vector3 targetUp = rotation * Vector3.up;


        //var newUp = Vector3.Slerp(currentUp, targetUp, dynamicStrength * Time.deltaTime);
        var newUp = Vector3.RotateTowards(currentUp, targetUp, staticStrength * Time.deltaTime, 0f);

        //Debug.Log(string.Format("fCount: {4}\nforward: {0}\ncUp: {1}\ntUp: {2}\nnUp: {3}", targetForward.ToString("G7"), currentUp.ToString("G7"), targetUp.ToString("G7"), newUp.ToString("G7"), Time.frameCount));
        //Debug.DrawRay(position, targetForward, Color.red, 0.1f);
        //Debug.DrawRay(position, currentUp, Color.green, 0.1f);
        //Debug.DrawRay(position, targetUp, Color.blue, 0.1f);
        //Debug.DrawRay(position, newUp, Color.yellow, 0.1f);

        return (position, Quaternion.LookRotation(targetForward, newUp));
    }

    protected override (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation)
    {
        return (position, rotation);
    }
}