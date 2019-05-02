using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Hitscan
{
    /// <summary>
    /// Cast a hitscan over the scene
    /// </summary>
    /// <param name="ray">The ray to cast</param>
    /// <param name="dHealth">The Delta Health object to use for damage/healing</param>
    /// <param name="distance">The distance to apply the hitscan over. Default, infinity</param>
    /// <param name="layerMask">The layers to cast the hitscan over. Default all</param>
    /// <param name="penetrate">This hitscan penetrates walls. Default false</param>
    /// <param name="targets">The targets to cast the hitscan over. Default, whole scene</param>
    /// <param name="OnHit">The function to call on a hit. Default None</param>
    public static void Cast(Ray ray, DeltaHealth dHealth, float distance = Mathf.Infinity, int layerMask = -1, bool penetrate = false, Collider[] targets = null, Action<Hurtbox, RaycastHit> OnHit = null)
        => Cast(ray.origin, ray.direction, dHealth, distance, layerMask, penetrate, targets, OnHit);

    /// <summary>
    /// Cast a hitscan over the scene
    /// </summary>
    /// <param name="origin">The point to cast the hitscan from</param>
    /// <param name="direction">The direction to cast the hitscan</param>
    /// <param name="dHealth">The Delta Health object to use for damage/healing</param>
    /// <param name="distance">The distance to apply the hitscan over. Default, infinity</param>
    /// <param name="layerMask">The layers to cast the hitscan over. Default all</param>
    /// <param name="penetrate">This hitscan penetrates walls. Default false</param>
    /// <param name="targets">The targets to cast the hitscan over. Default, whole scene</param>
    /// <param name="OnHit">The function to call on a hit. Default None</param>
    public static void Cast(Vector3 origin, Vector3 direction, DeltaHealth dHealth, float distance = Mathf.Infinity, int layerMask = -1, bool penetrate = false, Collider[] targets = null, Action<Hurtbox, RaycastHit> OnHit = null)
    {
        if (penetrate)
        {
            if(targets == null)
            {
                foreach (var hit in Physics.RaycastAll(origin, direction, distance, layerMask))
                {
                    ApplyCastHit(dHealth, hit, OnHit);
                }
            }
            else
            {
                foreach(var coll in targets)
                {
                    RaycastHit hit;
                    if(coll.Raycast(new Ray(origin, direction), out hit, distance))
                    {
                        ApplyCastHit(dHealth, hit, OnHit);
                    }
                }
            }
        }
        else
        {
            if(targets == null)
            {
                RaycastHit hit;
                if(Physics.Raycast(origin, direction, out hit, distance, layerMask))
                {
                    ApplyCastHit(dHealth, hit, OnHit);
                }
            }
            else
            {
                foreach (var coll in targets)
                {
                    RaycastHit hit;
                    if (coll.Raycast(new Ray(origin, direction), out hit, distance))
                    {
                        ApplyCastHit(dHealth, hit, OnHit);
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Check a raycast hit, and call appropriate event handlers.
    /// </summary>
    /// <param name="dHealth"></param>
    /// <param name="hit"></param>
    /// <param name="OnHit"></param>
    private static void ApplyCastHit(DeltaHealth dHealth, RaycastHit hit, Action<Hurtbox, RaycastHit> OnHit)
    {
        ApplyCastHit(hit.collider.GetComponent<Hurtbox>(), dHealth, hit, OnHit);
    }

    /// <summary>
    /// Check a raycast hit, and call appropriate event handlers
    /// </summary>
    /// <param name="hurtbox"></param>
    /// <param name="dHealth"></param>
    /// <param name="hit"></param>
    /// <param name="OnHit"></param>
    private static void ApplyCastHit(Hurtbox hurtbox, DeltaHealth dHealth, RaycastHit hit, Action<Hurtbox, RaycastHit> OnHit)
    {
        if(dHealth != null && hurtbox != null)
        {
            hurtbox.ApplyDeltaHealth(dHealth);
        }
        OnHit?.Invoke(hurtbox, hit);
    }
}