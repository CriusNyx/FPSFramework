using FPSFramework.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.DataStructures;

namespace FPSFramework.Demo
{
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
                Hitscan.Cast(transform.position, camera.transform.forward, HitscanType.damage, dHealth, layerMask: hitscanMask);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Hitscan.Cast(transform.position, camera.transform.forward, HitscanType.eventScan, cEvent: new UseEvent(), layerMask: hitscanMask);
            }
        }
    }
}