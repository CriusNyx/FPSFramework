using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control damage/healing applied to health controllers
/// Generally each hitbox/hitscan in the engine will need it's own health controller
/// </summary>
public class DeltaHealth
{
    /// <summary>
    /// Base ammound to damage/healing to apply
    /// </summary>
    public float baseDeltaHealth = 0f;

    /// <summary>
    /// Create a new delta health object
    /// </summary>
    public DeltaHealth()
    {

    }

    /// <summary>
    /// Create a delta health object with the specified health
    /// </summary>
    /// <param name="baseDeltaHealth"></param>
    public DeltaHealth(float baseDeltaHealth)
    {
        this.baseDeltaHealth = baseDeltaHealth;
    }

    /// <summary>
    /// Get the amount of delta health to apply.
    /// Override this method to allow for control over hit detection.
    /// </summary>
    /// <param name="health"></param>
    /// <returns></returns>
    public virtual float GetDelta(Health health)
    {
        return baseDeltaHealth;
    }
}
