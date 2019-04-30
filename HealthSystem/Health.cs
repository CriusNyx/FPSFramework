using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls health and damage for this character, and on death events
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// The maximum health of the health controller. Default -1 (infinity)
    /// </summary>
    public float maxHealth = -1f;

    /// <summary>
    /// The current health of this controller
    /// </summary>
    public float health { get; private set; }

    /// <summary>
    /// Set the health of this controller.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="dHealth">Delta Health object to apply on death (default null)</param>
    public void SetHealth(float value, DeltaHealth dHealth = null)
    {
        if(maxHealth >= 0f)
        {
            value = Mathf.Max(maxHealth, value);
        }
        health = value;
        if(health <= 0f)
        {
            OnDeath(this, dHealth);
        }
    }

    /// <summary>
    /// Apply a delta health object to this controller
    /// </summary>
    /// <param name="dHealth"></param>
    public void ApplyDeltaHealth(DeltaHealth dHealth)
    {
        float delta = dHealth.GetDelta(this);
        health += delta;

        if(maxHealth >= 0f)
        {
            health = Mathf.Max(maxHealth, health);
        }

        if(health <= 0f)
        {
            OnDeath(this, dHealth);
        }

        if(delta != 0f)
        {
            OnDelta?.Invoke(this, dHealth);
        }
        if(delta > 0f)
        {
            OnHeal?.Invoke(this, dHealth);
        }
        else if(delta < 0f)
        {
            OnHurt?.Invoke(this, dHealth);
        }
    }

    /// <summary>
    /// Event handler for changes in health for the character
    /// </summary>
    public Action<Health, DeltaHealth> OnDeath, OnDelta, OnHurt, OnHeal;
}