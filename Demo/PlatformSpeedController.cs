using GameEngine.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpeedController : MonoBehaviour
{
    int currentIndex;
    float sign;
    float[] arr = new float[] { 5, 180, 360, 700, 1000, 2000, 5000 };
    Spinner spinner;

    private void Start()
    {
        spinner = GetComponent<Spinner>();
        sign = Mathf.Sign(spinner.theta.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, arr.Length - 1);
            spinner.theta = Vector3.forward * sign * arr[currentIndex];
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, arr.Length - 1);
            spinner.theta = Vector3.forward * sign * arr[currentIndex];
        }
    }
}