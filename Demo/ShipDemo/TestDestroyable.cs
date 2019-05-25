using FPSFramework.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.Demo
{
    [RequireComponent(typeof(Health))]
    public class TestDestroyable : MonoBehaviour
    {
        Health health;

        private void Start()
        {
            health = GetComponent<Health>();
            health.SetHealth(1);
            health.OnDeath += (x, y) => Kill();

            Hurtbox.Create(gameObject, health);
        }

        void Kill()
        {
            Destroy(gameObject);
        }
    }
}