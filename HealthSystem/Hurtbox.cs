using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Apply this component to colliders to allow them to take health damage
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    /// <summary>
    /// Layer used for hurtboxes
    /// </summary>
    public const string HURTBOX_LAYER = "Hurtbox";

    /// <summary>
    /// Collider on this hurtbox
    /// </summary>
    new Collider collider;

    /// <summary>
    /// Health Controller
    /// </summary>
    public Health health;

    /// <summary>
    /// Create new hurtbox
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="health"></param>
    /// <returns></returns>
    public static Hurtbox Create(GameObject gameObject, Health health)
    {
        var output = gameObject.AddComponent<Hurtbox>();
        output.health = health;
        return output;
    }

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(HURTBOX_LAYER);

        collider = GetComponent<Collider>();
        //collider.isTrigger = true;
    }

    /// <summary>
    /// Apply delta health to this hurtbox
    /// </summary>
    /// <param name="dHealth"></param>
    public virtual void ApplyDeltaHealth(DeltaHealth dHealth)
    {
        health.ApplyDeltaHealth(dHealth);
    }
}