using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FPSFramework.Movement;
using FPSFramework.HealthSystem;

namespace FPSFramework.Actors
{
    /// <summary>
    /// A component for an actor
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Actor : MonoBehaviour
    {
        public MovementController movementController { get; private set; }
        public ViewController ViewController { get; private set; }
        public Health health { get; private set; }
        public PlatformController platformController { get; private set; }

        protected void InitializeActor(bool initializeColliders = true)
        {
            movementController = gameObject.AddComponent<MovementController>();
            ViewController = gameObject.AddComponent<ViewController>();
            health = gameObject.AddComponent<Health>();
            platformController = gameObject.AddComponent<PlatformController>();

            if (initializeColliders)
            {
                foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                {
                    Hurtbox.Create(collider.gameObject, health);
                }
            }
        }
    }
}