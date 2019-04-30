using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Movement;
using System;

/// <summary>
/// A component for an actor
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class Actor : MonoBehaviour
{
    public MovementController movementController { get; private set; }
    public ViewController viewController { get; private set; }
    public Health health { get; private set; }
    public PlatformController platformController { get; private set; }

    protected void InitializeActor(bool initializeColliders = true)
    {
        movementController = gameObject.AddComponent<MovementController>();
        viewController = gameObject.AddComponent<ViewController>();
        health = gameObject.AddComponent<Health>();
        platformController = gameObject.AddComponent<PlatformController>();

        if (initializeColliders)
        {
            foreach(var collider in gameObject.GetComponentsInChildren<Collider>())
            {
                Hurtbox.Create(collider.gameObject, health);
            }
        }
    }
}