using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : MonoBehaviour
{
    new Camera camera;

    static DeltaHealth dHealth = new DeltaHealth(-1);
    static Thunk<int> hitscanMask = new Thunk<int>(() => ~LayerMask.GetMask("Player"));

    private void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Hitscan.Cast(transform.position, camera.transform.forward, dHealth, layerMask: hitscanMask);
        }
    }
}