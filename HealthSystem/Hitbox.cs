using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.HealthSystem
{
    /// <summary>
    /// Allows a gameObject to collide with hurtboxes
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Hitbox : MonoBehaviour
    {
        const string HITBOX_LAYER = "Hitbox";

        new Collider collider;
        new Rigidbody rigidbody;

        DeltaHealth dHealth;

        /// <summary>
        /// Create a new hitbox on the gameObject with the DeltaHealth object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="dHealth"></param>
        /// <returns></returns>
        public static Hitbox Create(GameObject gameObject, DeltaHealth dHealth)
        {
            var hitbox = gameObject.AddComponent<Hitbox>();
            hitbox.dHealth = dHealth;
            return hitbox;
        }

        //private Hitbox Init(DeltaHealth dHealth)
        //{
        //    this.dHealth = dHealth;

        //    return this;
        //}

        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer(HITBOX_LAYER);

            collider = GetComponent<Collider>();
            collider.isTrigger = true;

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            hurtbox?.ApplyDeltaHealth(dHealth);
        }
    }
}